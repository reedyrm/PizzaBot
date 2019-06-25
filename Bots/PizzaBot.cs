using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace PizzaBot.Bots
{
    public class PizzaBot<T> : BasePizzaBot<T> where T : Dialog
    {
        public PizzaBot(ConversationState conversationState, UserState userState, T dialog, ILogger<BasePizzaBot<T>> logger) 
            : base(conversationState, userState, dialog, logger)
        {
        }

        protected override async Task OnMembersAddedAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = MessageFactory.Text($"Hello {member.Name} and Welcome to Pizza Bot. " +
                        "Type anything to get started.");
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }
    }
}
