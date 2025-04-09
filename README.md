
# RobloxUserOnlineTracker

**RobloxUserOnlineTracker** is a lightweight .NET library that allows you to track the **online presence of Roblox users in real-time**.

> ⚠️ This project is not affiliated with Roblox Corporation. Use at your own risk and comply with Roblox's Terms of Service.

---

## ✨ Features

- ✅ Track multiple Roblox user IDs for presence changes.
- ⏱️ Set custom tracking intervals.
- 🪝 Event-driven architecture with two key events:
  - `UserOnlinePresenceChanged` — triggered when a user's online status changes.
  - `TrackingErrorOccurred` — triggered when an error occurs during tracking.
- 👍 Easy to use.

---

## 🚀 Getting Started

### Installation

Install the NuGet package:

```bash
dotnet add package RobloxUserOnlineTracker
```

Or via the [NuGet Gallery](https://www.nuget.org/packages/RobloxUserOnlineTracker).

---

### Usage

#### Tracking example

```csharp
using RobloxUserOnlineTracker;

var tracker = new RobloxUserOnlineTrackerClient(".ROBLOSECURITY_COOKIE_HERE");

tracker.UserOnlinePresenceChanged += (sender, args) =>
{
    Console.WriteLine($"User {args.UserPresence.User.Id} is now {args.UserPresence.UserPresence}");
};

tracker.TrackingErrorOccurred += (sender, ex) =>
{
    Console.WriteLine($"Error occurred: {ex.Message}");
};

long[] userIdsToTrack = { 123456, 789012 };
tracker.StartTracking(userIdsToTrack, trackInterval: 10000); // Every 10 seconds
```

#### Onetime call example

```csharp
using RobloxUserOnlineTracker;

var tracker = new RobloxUserOnlineTrackerClient(".ROBLOSECURITY_COOKIE_HERE");

long[] userIds = { 123456, 789012 };
var usersStatusOnline = await tracker.GetUserOnlinePresenceAsync(userIds);
```

### Real use case (old version, the code may changes)

In the following screenshots I have included a real use case. I decide to track the online status of myself and another test account and send a real-time notification to myself with Telegram using a bot. Finally I started the application in a Docker container.

![Screenshot 1](https://camo.githubusercontent.com/6d507dae481cda4113b2b4afb1d3d6ad7caab0b70ed7395d5561a3eaef03e3b1/68747470733a2f2f692e706f7374696d672e63632f4d5a736b3958374d2f72756f742d312e77656270)

![Screenshot 2](https://camo.githubusercontent.com/c9708f3f27fd751c48183ff3e49d71e650e47d3a9ee19c2e583b24c4524c044e/68747470733a2f2f692e706f7374696d672e63632f66545847443534582f72756f742d322e77656270)

---

## 📦 API Reference

### `RobloxUserOnlineTrackerClient` - *instantiable* - *disposable*

| Member | Description |
|--------|-------------|
| `RobloxUserOnlineTrackerClient(string cookieValue)` | Initializes the tracker with a Roblox `.ROBLOSECURITY` cookie. |
| `void Dispose()` | Releases the resources used by the client instance. |
| `void StartTracking(long[] userIds, int trackInterval = 5000, CancellationToken cancellationToken = default)` | Begins polling user presence every N milliseconds (minimum 5000). |
| `void StopTracking()` | Stops polling and clears tracked data. |
| `Task<RobloxUserOnlinePresence[]> GetUserOnlinePresenceAsync(long[] userIds)` | Manually query user presence without event logic. |
| `event UserOnlinePresenceChanged` | Fired when a user's online status changes. |

### `RobloxUserOnlinePresence` - *not instantiable* - *readonly fields* - *returned*

| Member | Description |
|--------|-------------|
| `RobloxUser User` | Get the user informations as a `RobloxUser` object. |
| `UserPresenceType UserPresence` | Gets the actual presence of the user. |
| `string CurrentLocation` | Get the location name where the user is currently located. This information is only available if the user shares this information with you or publicly. |
| `long? GameId` | Get the location ID where the user is currently located. This information is only available if the user shares this information with you or publicly. |
| `Guid? GameInstanceId` | Get the instance ID of the location where the user is currently located. It can be used to join the user experience. This information is only available if the user shares this information with you or publicly. |

### `RobloxUser` - *not instantiable* - *readonly fields*

| Member | Description |
|--------|-------------|
| `string About` | Get the user about description. |
| `DateTime Created` | Get the date and time when the user was created. |
| `bool IsBanned` | Get if the user is banned. |
| `bool HasVerifiedBadge` | Get if the user has the verified badge. |
| `long Id` | Get the user ID. |
| `string Username` | Get the user name. |
| `string? DisplayName` | Get the user display name. |

---

## 🛡 License

Licensed under the **Mozilla Public License 2.0** (MPL-2.0).  
You are free to use, modify, and distribute the source, provided that modified files are disclosed.

🔗 https://mozilla.org/MPL/2.0/

---

## 👨‍💻 Author

**Marco Concas**  
[GitHub](https://github.com/Marko97IT)

---

## ❤️ Support & Contributions

Pull requests and issues are welcome!  
If you find this project useful, consider leaving a ⭐ on the repository.
