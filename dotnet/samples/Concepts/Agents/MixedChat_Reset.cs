﻿// Copyright (c) Microsoft. All rights reserved.
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Agents;

/// <summary>
/// Demonstrate <see cref="ChatCompletionAgent"/> agent interacts with
/// <see cref="OpenAIAssistantAgent"/> when it produces image output.
/// </summary>
public class MixedChat_Reset(ITestOutputHelper output) : BaseTest(output)
{
    private const string CountingInstructions =
        """
        The user may either provide information or query on information previously provided.
        If the query does not correspond with information provided, inform the user that their query cannot be answered.
        """;

    [Fact]
    public async Task ResetChatAsync()
    {
        OpenAIFileService fileService = new(TestConfiguration.OpenAI.ApiKey);

        // Define the agents
        OpenAIAssistantAgent assistantAgent =
            await OpenAIAssistantAgent.CreateAsync(
                kernel: new(),
                config: new(this.ApiKey, this.Endpoint),
                new()
                {
                    Name = nameof(OpenAIAssistantAgent),
                    Instructions = CountingInstructions,
                    ModelId = this.Model,
                });

        ChatCompletionAgent chatAgent =
            new()
            {
                Name = nameof(ChatCompletionAgent),
                Instructions = CountingInstructions,
                Kernel = this.CreateKernelWithChatCompletion(),
            };

        // Create a chat for agent interaction.
        AgentGroupChat chat = new();

        // Respond to user input
        try
        {
            await InvokeAgentAsync(assistantAgent, "What is my favorite color?");
            await InvokeAgentAsync(chatAgent);

            await InvokeAgentAsync(assistantAgent, "I like green.");
            await InvokeAgentAsync(chatAgent);

            await InvokeAgentAsync(assistantAgent, "What is my favorite color?");
            await InvokeAgentAsync(chatAgent);

            //await DisplayHistory(assistantAgent);
            //await DisplayHistory(chatAgent);

            await chat.ResetAsync();

            await InvokeAgentAsync(assistantAgent, "What is my favorite color?");
            await InvokeAgentAsync(chatAgent);

            //await DisplayHistory(assistantAgent);
            //await DisplayHistory(chatAgent);
        }
        finally
        {
            await chat.ResetAsync();
            await assistantAgent.DeleteAsync();
        }

        // Local function to invoke agent and display the conversation messages.
        async Task InvokeAgentAsync(Agent agent, string? input = null)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                chat.AddChatMessage(new(AuthorRole.User, input));
                Console.WriteLine($"\n# {AuthorRole.User}: '{input}'");
            }

            await foreach (ChatMessageContent message in chat.InvokeAsync(agent))
            {
                if (!string.IsNullOrWhiteSpace(message.Content))
                {
                    Console.WriteLine($"\n# {message.Role} - {message.AuthorName ?? "*"}: '{message.Content}'");
                }
            }
        }

        async Task DisplayHistory(Agent agent)
        {
            Console.WriteLine("\n\n>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine($">>>> HISTORY: {agent.Name} ");
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

            await foreach (ChatMessageContent content in chat.GetChatMessagesAsync(agent).Reverse())
            {
                Console.WriteLine($">>>> {content.Role} - {content.AuthorName ?? "*"}: '{content.Content}'");
            }
        }
    }
}
