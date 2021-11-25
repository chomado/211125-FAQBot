// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.14.0

using Azure;
using Azure.AI.Language.QuestionAnswering;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot7.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly IConfiguration configuration;

        public EchoBot(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var endpoint = new Uri(configuration["Endpoint"]);
            var credential = new AzureKeyCredential(configuration["Key"]);
            var projectName = configuration["ProjectName"];
            var deploymentName = "production";

            var client = new QuestionAnsweringClient(endpoint, credential);
            var project = new QuestionAnsweringProject(projectName, deploymentName);

            var options = new AnswersOptions(); 
            options.ConfidenceThreshold = 0.10; 

            Response<AnswersResult> response = await client.GetAnswersAsync(turnContext.Activity.Text, project, options);

            var replyText = response.Value.Answers.First().Answer;
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text($"確信度： {response.Value.Answers.First().Confidence * 100:00.00}％", replyText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
