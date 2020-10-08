using System.Collections.Generic;
using form_builder.Models;
using form_builder.Models.Elements;

namespace form_builder.Helpers.ElementHelpers
{
    public interface IElementHelper
    {
        string CurrentValue(Element element, Dictionary<string, dynamic> viewModel, FormAnswers answers, string pageSlug, string guid, string suffix = "");

        T CurrentValue<T>(Element element, Dictionary<string, dynamic> viewModel, FormAnswers answers, string pageSlug, string guid, string suffix = "") where T : new();
        
        bool CheckForQuestionId(Element element);
        
        bool CheckForLabel(Element element);
        
        bool CheckForMaxLength(Element element);
        
        bool CheckIfLabelAndTextEmpty(Element element);
        
        bool CheckForRadioOptions(Element element);
        
        bool CheckForSelectOptions(Element element);
        
        bool CheckForCheckBoxListValues(Element element);
        
        bool CheckAllDateRestrictionsAreNotEnabled(Element element);
        
        void ReSelectPreviousSelectedOptions(Element element);
        
        void ReCheckPreviousRadioOptions(Element element);
        
        bool CheckForProvider(Element element);
        
        object GetFormDataValue(string guid, string key);
        
        FormAnswers GetFormData(string guid);
        
        List <PageSummary> GenerateQuestionAndAnswersList(string guid, FormSchema formSchema);

        string GenerateDocumentUploadUrl(Element element, FormSchema formSchema, FormAnswers formAnswers);
    }
}