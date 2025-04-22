using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;

namespace chat.Components.Pages;

public partial class Home
{
    private HubConnection? hubConnection;
    private string userInput = "User";
    private string messageInput;
    private List<(string User, string Text)> messages = new();

    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
            .WithAutomaticReconnect()
            .Build();

        hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            messages.Add((user, message));
            InvokeAsync(StateHasChanged);
        });

        await hubConnection.StartAsync();
    }

    private async Task SendMessage()
    {
        if (!string.IsNullOrWhiteSpace(messageInput))
        {
            await hubConnection.SendAsync("SendMessage", userInput, messageInput);
            messageInput = string.Empty;
        }
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SendMessage();
        }
    }
}