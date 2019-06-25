using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PizzaBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;


namespace PizzaBot.Bots
{
    public class PizzaDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserData> _userDataAccessor;
        private readonly IStatePropertyAccessor<ConversationData> _conversationDataAccessor;

        private static string TOP_LEVEL_WATERFALL_NAME = "INITIAL";
        private static String NUM_PIZZA_DIALOG_PROMPT_NAME = "NUM_PIZZA_PROMPT";

        public PizzaDialog(UserState userState, ConversationState conversationState)
            : base(nameof(PizzaDialog))
        {
            _userDataAccessor = userState.CreateProperty<UserData>("UserData");
            _conversationDataAccessor = conversationState.CreateProperty<ConversationData>("ConversationData");


            var topLevelWaterfallSteps = new WaterfallStep[]
            {
                StartAsync
            };

            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                TakeoutOrDeliveryStepAsync,
                PizzaTypeStepAsync,
                PizzaSizeStepAsync,
                NumberOfPizzasStepAsync,
                ConfirmOrderStepAsync,
                PlaceOrderStepAsync
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(TOP_LEVEL_WATERFALL_NAME, waterfallSteps));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(NUM_PIZZA_DIALOG_PROMPT_NAME, NumPizzaValidator));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            // The initial child Dialog to run.
            InitialDialogId = TOP_LEVEL_WATERFALL_NAME;
        }

        private static async Task<DialogTurnResult> StartAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(WaterfallDialog), null, cancellationToken); 
        }


        private static async Task<DialogTurnResult> TakeoutOrDeliveryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Takeout or Delivery?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Delivery", "Takeout"}),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> PizzaTypeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["takeoutOrDelivery"] = ((FoundChoice)stepContext.Result).Value;

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What type of pizza would do you want?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Cheese", "Pepperoni", "Meat Lovers", "Veggie Delight" }),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> PizzaSizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["pizzaType"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What size?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Small", "Medium", "Large", "Extra Large" }),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> NumberOfPizzasStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["pizzaSize"] = ((FoundChoice)stepContext.Result).Value;


            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"How many {stepContext.Values["pizzaSize"]} {stepContext.Values["pizzaType"]} pizzas would you like?"),
                RetryPrompt = MessageFactory.Text("The value entered must be greater than 0 and less than 10."),
            };

            return await stepContext.PromptAsync(NUM_PIZZA_DIALOG_PROMPT_NAME, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmOrderStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["numPizza"] = (int)stepContext.Result;

            var numberOfPizzas = stepContext.Values["numPizza"];
            var pizzaSize = stepContext.Values["pizzaSize"];
            var pizzaType = stepContext.Values["pizzaType"];

            // We can send messages to the user at any point in the WaterfallStep.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Order Preview\n{numberOfPizzas} - {pizzaSize} {pizzaType}"), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Does your order look correct?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> PlaceOrderStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //If user typed No, we want to move back to the beginning
            if (!(bool)stepContext.Result) {
                return await stepContext.ReplaceDialogAsync(TOP_LEVEL_WATERFALL_NAME, null, cancellationToken);
            }

            //If user types Yes, we want to place the order

            var newOrder = new Order() { DeliveryMethod = (String)stepContext.Values["takeoutOrDelivery"] };
            newOrder.OrderedPizzas.Add(new Pizza()
            {
                NumberOfPizzas = (int)stepContext.Values["numPizza"],
                PizzaType = (String)stepContext.Values["pizzaType"],
                PizzaSize = (String)stepContext.Values["pizzaSize"]
            });

            // Make a call to the Order API

            // Save Completed Order in the UserProfile on success
            var userProfile = await _userDataAccessor.GetAsync(stepContext.Context, null, cancellationToken);

            userProfile.Orders.Add(newOrder);

            await _userDataAccessor.SetAsync(stepContext.Context, userProfile, cancellationToken);

            await stepContext.Context.SendActivityAsync("Thank you for your order!");

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private static Task<bool> NumPizzaValidator(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            // This condition is our validation rule. You can also change the value at this point.
            return Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value > 0 && promptContext.Recognized.Value < 10);
        }
    }
}
