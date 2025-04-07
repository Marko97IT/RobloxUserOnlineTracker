# RobloxUserOnlineTracker

**RobloxUserOnlineTracker** is a lightweight .NET library that allows you to track the **online presence of Roblox users in real-time**.

> ⚠️ This project is not affiliated with Roblox Corporation. Use at your own risk and comply with Roblox's Terms of Service.

---

## ✨ Features

- ✅ Track multiple Roblox user IDs for presence changes.
- ⏱️ Set custom tracking intervals.
- 🪝 Event-driven architecture with two key events:
  - `UserStatusChanged` — triggered when a user's online status changes.
  - `RobloxCookieExpired` — triggered when the session becomes unauthorized.
- 👍 Easy to use and highly extensible in your projects.

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

var tracker = new TrackerOnlineStatusClient(".ROBLOSECURITY_COOKIE_HERE");

tracker.UserStatusChanged += (sender, args) =>
{
    Console.WriteLine($"User {args.UserId} is now {args.UserStatus}");
};

tracker.RobloxCookieExpired += (sender, args) =>
{
    Console.WriteLine("Roblox cookie expired. Stopping tracker.");
};

long[] userIdsToTrack = [123456, 789012];
tracker.StartTracking(userIdsToTrack, trackInterval: 10000); // Every 10 seconds
```

#### Onetime call example

```csharp
using RobloxUserOnlineTracker;

var tracker = new TrackerOnlineStatusClient(".ROBLOSECURITY_COOKIE_HERE");

long[] userIds = [123456, 789012];
var usersStatusOnline = await tracker.GetUsersOnlineStatusAsync(userIds);
```

---

## 📦 API Reference

### `TrackerOnlineStatusClient`

| Member | Description |
|--------|-------------|
| `TrackerOnlineStatusClient(string cookieValue)` | Initializes the tracker with a Roblox `.ROBLOSECURITY` cookie. |
| `void Dispose()` | Releases the resources used by the client instance. |
| `void StartTracking(long[] userIds, int trackInterval = 5000)` | Begins polling user presence every N milliseconds. |
| `void StopTracking()` | Stops polling and clears tracked data. |
| `Task<UsersOnlineStatus> GetUsersOnlineStatusAsync(long[] userIds)` | Manually query user presence without event logic. |
| `event UserStatusChanged` | Fired when a user's status changes. |
| `event RobloxCookieExpired` | Fired when the cookie becomes unauthorized. |

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
