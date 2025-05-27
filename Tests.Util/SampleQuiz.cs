using Bogus;
using QuizWorld.Common.Constants.Types;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Infrastructure.Data.Entities.Quiz;

namespace Tests.Util
{
    public static class SampleQuiz
    {
        public static Quiz GenerateQuiz(int version, bool instantMode, bool isDeleted)
        {
            return new Faker<Quiz>()
                .RuleFor(quiz => quiz.Id, faker => faker.Random.Number(1, 100_000_000))
                .RuleFor(quiz => quiz.NormalizedTitle, faker => faker.Lorem.Sentence(5))
                .RuleFor(quiz => quiz.Description, faker => faker.Lorem.Sentence(7))
                .RuleFor(quiz => quiz.Version, version)
                .RuleFor(quiz => quiz.InstantMode, instantMode)
                .RuleFor(quiz => quiz.CreatedOn, DateTime.Now)
                .RuleFor(quiz => quiz.UpdatedOn, DateTime.Now)
                .RuleFor(quiz => quiz.Questions,
                    [
                        SampleQuestions.GenerateQuestion(1, QuestionTypes.SingleChoice),
                        SampleQuestions.GenerateQuestion(1, QuestionTypes.MultipleChoice),
                        SampleQuestions.GenerateQuestion(1, QuestionTypes.Text),
                        SampleQuestions.GenerateQuestion(version, QuestionTypes.SingleChoice),
                        SampleQuestions.GenerateQuestion(version, QuestionTypes.MultipleChoice),
                        SampleQuestions.GenerateQuestion(version, QuestionTypes.Text)
                    ])
                .RuleFor(quiz => quiz.IsDeleted, isDeleted)
                .RuleFor(quiz => quiz.CreatorId, Guid.NewGuid())
                .RuleFor(quiz => quiz.Creator, faker => new ApplicationUser()
                {
                    UserName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female),
                })
                .Generate();
        }
    }
}
