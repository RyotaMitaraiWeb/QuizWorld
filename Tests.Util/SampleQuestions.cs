using Bogus;
using QuizWorld.Common.Constants.Types;
using QuizWorld.Infrastructure.Data.Entities.Quiz;

namespace Tests.Util
{
    public class SampleQuestions
    {
        public const int SampleSize = 10;

        public static Question GenerateQuestion(int version, QuestionTypes type)
        {
            if (type == QuestionTypes.Text)
            {
                return GenerateTextQuestion(version);
            }

            if (type == QuestionTypes.SingleChoice)
            {
                return GenerateSingleChoiceQuestion(version);
            }

            return GenerateMultipleChoiceQuestion(version);
        }

        private static Question GenerateMultipleChoiceQuestion(int version)
        {
            Answer a1 = SampleAnswers.GenerateAnswer(true);
            Answer a2 = SampleAnswers.GenerateAnswer(false);
            Answer a3 = SampleAnswers.GenerateAnswer(false);
            Answer a4 = SampleAnswers.GenerateAnswer(true);
            return new Faker<Question>()
                .RuleFor(question => question.Id, Guid.NewGuid())
                .RuleFor(question => question.Prompt, faker => faker.Random.Words())
                .RuleFor(question => question.Answers, [a1, a2, a3, a4])
                .RuleFor(question => question.QuestionType, GenerateQuestionType(QuestionTypes.MultipleChoice))
                .RuleFor(question => question.Notes, string.Empty)
                .RuleFor(question => question.Version, version)
                .Generate();
        }
        private static Question GenerateSingleChoiceQuestion(int version)
        {
            Answer a1 = SampleAnswers.GenerateAnswer(true);
            Answer a2 = SampleAnswers.GenerateAnswer(false);
            Answer a3 = SampleAnswers.GenerateAnswer(false);
            return new Faker<Question>()
                .RuleFor(question => question.Id, Guid.NewGuid())
                .RuleFor(question => question.Prompt, faker => faker.Random.Words())
                .RuleFor(question => question.Answers, [a1, a2, a3])
                .RuleFor(question => question.QuestionType, GenerateQuestionType(QuestionTypes.SingleChoice))
                .RuleFor(question => question.Notes, string.Empty)
                .RuleFor(question => question.Version, version)
                .Generate();
        }
        private static Question GenerateTextQuestion(int version)
        {
            Answer a1 = SampleAnswers.GenerateAnswer(true);
            Answer a2 = SampleAnswers.GenerateAnswer(true);
            Answer a3 = SampleAnswers.GenerateAnswer(true);
            return new Faker<Question>()
                .RuleFor(question => question.Id, Guid.NewGuid())
                .RuleFor(question => question.Prompt, faker => faker.Random.Words())
                .RuleFor(question => question.Answers, [a1, a2, a3])
                .RuleFor(question => question.QuestionType, GenerateQuestionType(QuestionTypes.Text))
                .RuleFor(question => question.Notes, string.Empty)
                .RuleFor(question => question.Version, version)
                .Generate();
        }

        private static QuestionType GenerateQuestionType(QuestionTypes type)
        {
            switch (type)
            {
                case QuestionTypes.Text:
                    return new QuestionType()
                    {
                        FullName = QuestionTypesFullNames.Text,
                        ShortName = QuestionTypesShortNames.Text,
                        Id = 1,
                        Type = type,
                    };
                case QuestionTypes.MultipleChoice:
                    return new QuestionType()
                    {
                        FullName = QuestionTypesFullNames.MultipleChoice,
                        ShortName = QuestionTypesShortNames.MultipleChoice,
                        Id = 2,
                        Type = type,
                    };
                case QuestionTypes.SingleChoice:
                    return new QuestionType()
                    {
                        FullName = QuestionTypesFullNames.SingleChoice,
                        ShortName = QuestionTypesShortNames.SingleChoice,
                        Id = 3,
                        Type = type,
                    };
                default: throw new Exception("Invalid type");
            }
        }
    }
}
