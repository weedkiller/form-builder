﻿using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using form_builder.Extensions;
using form_builder.Gateways;
using form_builder.Models.Elements;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace form_builder.Providers.Transforms.ReusableElements
{
    public class S3ReusableElementTransformDataProvider : IReusableElementTransformDataProvider
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public S3ReusableElementTransformDataProvider(IS3Gateway s3Service, ILogger<S3ReusableElementTransformDataProvider> logger, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _s3Gateway = s3Service;
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<IElement> Get(string schemaName)
        {
            try
            {
                var s3Result = await _s3Gateway.GetObject(_configuration["S3BucketKey"], $"{_environment.EnvironmentName.ToS3EnvPrefix()}/Elements/{schemaName}.json");

                using (Stream responseStream = s3Result.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    var responseBody = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<Element>(responseBody);
                }
            }
            catch (AmazonS3Exception e)
            {
                var ex = new Exception($"S3ReusableElementTransformDataProvider: An error has occured while attempting to get S3 Object, Exception: {e.Message}. {_environment.EnvironmentName.ToS3EnvPrefix()}/Elements/{schemaName} ", e);
                throw ex;
            }
            catch (Exception e)
            {
                var ex = new Exception($"S3ReusableElementTransformDataProvider: An error has occured while attempting to deserialise object, Exception: {e.Message}", e);
                throw ex;
            }
        }
    }
}