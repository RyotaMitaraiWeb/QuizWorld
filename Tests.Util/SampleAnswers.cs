using Bogus;
using QuizWorld.Infrastructure.Data.Entities.Quiz;

namespace Tests.Util
{
    public static class SampleAnswers
    {
        public static Answer GenerateAnswer(bool correct)
        {
            return new Faker<Answer>()
                .RuleFor(answer => answer.Id, Guid.NewGuid())
                .RuleFor(answer => answer.Correct, correct)
                .RuleFor(answer => answer.Value, faker => faker.Lorem.Word())
                .Generate();
        }
    }
}
