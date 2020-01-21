﻿using form_builder.Mappers;
using form_builder.Models;
using form_builder.Models.Elements;
using form_builder.Providers.SchemaProvider;
using form_builder.Providers.StorageProvider;
using form_builder.Services.MappingService.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace form_builder.Services.MappingService
{
    public interface IMappingService
    {
        Task<MappingEntity> Map(string sessionGuid, string form);
    }

    public class MappingService : IMappingService
    {
        private readonly ISchemaProvider _schemaProvider;
        private readonly IDistributedCacheWrapper _distributedCache;

        public MappingService(ISchemaProvider schemaProvider, IDistributedCacheWrapper distributedCache)
        {
            _schemaProvider = schemaProvider;
            _distributedCache = distributedCache;
        }

        public async Task<MappingEntity> Map(string sessionGuid, string form)
        {
            var baseForm = await _schemaProvider.Get<FormSchema>(form);
            var formData = _distributedCache.GetString(sessionGuid);
            var convertedAnswers = JsonConvert.DeserializeObject<FormAnswers>(formData);
            convertedAnswers.FormName = form;

            return new MappingEntity
            {
                Data = CreatePostData(convertedAnswers, baseForm),
                BaseForm = baseForm,
                FormAnswers = convertedAnswers
            };
        }

        private object CreatePostData(FormAnswers formAnswers, FormSchema formSchema)
        {
            var data = new ExpandoObject() as IDictionary<string, object>;

            formSchema.Pages.SelectMany(_ => _.ValidatableElements)
                .ToList()
                .ForEach(_ => data = RecursiveCheckAndCreate(string.IsNullOrEmpty(_.Properties.TargetMapping) ? _.Properties.QuestionId : _.Properties.TargetMapping, _, formAnswers, data));

            return data;
        }

        private IDictionary<string, object> RecursiveCheckAndCreate(string targetMapping, IElement element, FormAnswers formAnswers, IDictionary<string, object> obj)
        {
            var splitTargets = targetMapping.Split(".");

            if (splitTargets.Length == 1)
            {
                object objectValue;
                if (obj.TryGetValue(splitTargets[0], out objectValue))
                {
                    var combinedValue = $"{objectValue} {ElementMapper.GetAnswerValue(element, formAnswers)}";
                    obj.Remove(splitTargets[0]);
                    obj.Add(splitTargets[0], combinedValue);
                    return obj;
                }

                obj.Add(splitTargets[0], ElementMapper.GetAnswerValue(element, formAnswers));
                return obj;
            }

            object subObject;
            if (!obj.TryGetValue(splitTargets[0], out subObject))
                subObject = new ExpandoObject();

            subObject = RecursiveCheckAndCreate(targetMapping.Replace($"{splitTargets[0]}.", ""), element, formAnswers, subObject as IDictionary<string, object>);

            obj.Remove(splitTargets[0]);
            obj.Add(splitTargets[0], subObject);

            return obj;
        }
    }
}