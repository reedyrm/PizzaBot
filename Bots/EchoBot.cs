// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio v4.3.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using PizzaBot.Models;

namespace PizzaBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private BotState _conversationState;
        private BotState _userState;

        public EchoBot(ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Get the state properties from the turn context.

            var conversationStateAccessors = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationData());

            var userStateAccessors = _userState.CreateProperty<UserData>(nameof(UserData));
            var userProfile = await userStateAccessors.GetAsync(turnContext, () => new UserData());


            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            // Get the state properties from the turn context.

            var conversationStateAccessors = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationData());

            var userStateAccessors = _userState.CreateProperty<UserData>(nameof(UserData));
            var userProfile = await userStateAccessors.GetAsync(turnContext, () => new UserData());



            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and Welcome!"), cancellationToken);
                }
            }
        }

        /**
         * Save the ConversationData and UserProfile
         */ 
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }


    }
}
