using System.Linq;
using System.Text.RegularExpressions;
using form_builder.Models;
using form_builder.Services.RetrieveExternalDataService.Entities;

namespace form_builder.Helpers.ActionsHelpers
{
    public interface IActionHelper
    {
        ExternalDataEntity GenerateUrl(string baseUrl, FormAnswers formAnswers);

        string InsertFormAnswersIntoProperty(FormAction action, FormAnswers formAnswers);

        string InsertFormAnswersIntoContent(FormAction action, FormAnswers formAnswers);

        string InsertFormAnswersIntoParameters(FormAction action, FormAnswers formAnswers);
    }

    public class ActionHelper : IActionHelper
    {
        private Regex _tagRegex => new Regex("(?<={{).*?(?=}})", RegexOptions.Compiled);

        public ExternalDataEntity GenerateUrl(string baseUrl, FormAnswers formAnswers)
        {
            var matches = _tagRegex.Matches(baseUrl);
            var newUrl = matches.Aggregate(baseUrl, (current, match) => Replace(match, current, formAnswers));
            return new ExternalDataEntity
            {
                Url = newUrl,
                IsPost = !matches.Any()
            };
        }

        public string InsertFormAnswersIntoProperty(FormAction action, FormAnswers formAnswers)
        {
            var matches = _tagRegex.Matches(action.Properties.To).ToList();

            var questionIdList = matches
                .Select(match => RecursiveGetAnswerValue(match.Value, formAnswers.Pages
                    .SelectMany(_ => _.Answers)
                    .FirstOrDefault(_ => _.QuestionId.Equals(match.Value))))
                .ToList();

            questionIdList.AddRange(action.Properties.To.Split(",").Where(_ => !_tagRegex.IsMatch(_)));

            return questionIdList.Aggregate("", (questionId, answer) => questionId + answer + ",");
        }

        public string InsertFormAnswersIntoContent(FormAction action, FormAnswers formAnswers)
        {
            var matches = _tagRegex.Matches(action.Properties.Content).ToList();

            var content = matches.Aggregate(action.Properties.Content, (current, match) => Replace(match, current, formAnswers));

            return content;
        }

        public string InsertFormAnswersIntoParameters(FormAction action, FormAnswers formAnswers)
        {
            var matches = _tagRegex.Matches(action.Properties.Content).ToList();

            var questionIdList = matches
                .Select(match => RecursiveGetAnswerValue(match.Value, formAnswers.Pages
                    .SelectMany(_ => _.Answers)
                    .FirstOrDefault(_ => _.QuestionId.Equals(match.Value))))
                .ToList();

            questionIdList.AddRange(action.Properties.To.Split(",").Where(_ => !_tagRegex.IsMatch(_)));

            return questionIdList.Aggregate("", (questionId, answer) => questionId + answer + ",");
        }

        private string Replace(Match match, string current, FormAnswers formAnswers)
        {
            var splitTargets = match.Value.Split(".");
            var answer = RecursiveGetAnswerValue(match.Value, formAnswers.Pages.SelectMany(_ => _.Answers).First(a => a.QuestionId.Equals(splitTargets[0])));

            return current.Replace($"{{{{{match.Groups[0].Value}}}}}", answer);
        }

        private string RecursiveGetAnswerValue(string targetMapping, Answers answer)
        {
            var splitTargets = targetMapping.Split(".");

            if (splitTargets.Length == 1)
                return (dynamic)answer.Response;

            var subObject = new Answers { Response = (dynamic)answer.Response[splitTargets[1]] };
            return RecursiveGetAnswerValue(targetMapping.Replace($"{splitTargets[0]}.", string.Empty), subObject);
        }
    }
}