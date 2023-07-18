﻿using Microsoft.EntityFrameworkCore;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.Web.Contracts.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Services.GradeService
{
    /// <summary>
    /// A service for retrieving correct answers of quizzes or questions.
    /// </summary>
    public class GradeService : IGradeService
    {
        private readonly IRepository repository;
        public GradeService(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets the correct answers for the question with the given <paramref name="questionId"/> and the given <paramref name="version"/>.
        /// Throws InvalidOperationException if the quiz is not in instant mode.
        /// </summary>
        /// <param name="questionId">The ID of the question to be graded</param>
        /// <param name="version">The version of the question whose answers will be retrieved</param>
        /// <returns>A view model of a graded question or null if the question does not exist or the quiz is deleted</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<GradedQuestionViewModel?> GetCorrectAnswersForQuestionById(Guid questionId, int version)
        {
            var question = await this.repository
                .AllReadonly<Question>()
                .Where(q => q.Id == questionId && q.Version == version && !q.Quiz.IsDeleted)
                .Select(q => new GradedQuestionViewModel()
                {
                    Id = q.Id.ToString(),
                    Answers = q.Answers
                        .Where(a => a.Correct)
                        .Select(a => new AnswerViewModel()
                        {
                            Id = a.Id.ToString(),
                            Value = a.Value,
                        }),
                    InstantMode = q.Quiz.InstantMode,
                })
                .FirstOrDefaultAsync();

            if (question == null)
            {
                return null;
            }

            if (!question.InstantMode)
            {
                throw new InvalidOperationException("This question cannot be graded individually because the quiz is not in instant mode");
            }

            return question;
        }

        /// <summary>
        /// Gets the correct answers for the question with the given <paramref name="questionId"/> and the given <paramref name="version"/>.
        /// Throws InvalidOperationException if the quiz is not in instant mode.
        /// </summary>
        /// <param name="questionId">The ID of the question to be graded</param>
        /// <param name="version">The version of the question whose answers will be retrieved</param>
        /// <returns>A view model of a graded question or null if the question does not exist or the quiz is deleted</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<GradedQuestionViewModel?> GetCorrectAnswersForQuestionById(string questionId, int version)
        {
            bool isGuid = Guid.TryParse(questionId, out Guid id);
            if (!isGuid)
            {
                throw new ArgumentException("questionId is an invalid GUID");
            }

            return await this.GetCorrectAnswersForQuestionById(id, version);
        }

        public Task<IEnumerable<GradedQuestionViewModel>> GetCorrectAnswersForQuestionsByQuizId(int quizId, int version)
        {
            throw new NotImplementedException();
        }
    }
}
