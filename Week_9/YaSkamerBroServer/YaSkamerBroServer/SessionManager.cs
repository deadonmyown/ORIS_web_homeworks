﻿using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaSkamerBroServer
{
    public static class SessionManager
    {

        private static MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public static Session CreateOrGetSession(object key, Func<Session> createSession)
        {
            Session session;
            if (_memoryCache.TryGetValue(key, out session))
            {
                session = createSession();
                var memoryCacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(2));
                _memoryCache.Set(key, session, memoryCacheOptions);
            }
            return session;
        }

        public static bool CheckSession(object key) => _memoryCache.TryGetValue(key, out _);

        public static Session? GetSession(object key) => _memoryCache.TryGetValue(key, out Session session) ? session : null;
    }

    public struct Session
    {
        public Guid Id { get; }
        public int AccountId { get; }
        public string Email { get; }
        public DateTime CreateDateTime { get; }

        public Session(Guid id, int accountId, string email, DateTime createDateTime)
        {
            Id = id;
            AccountId = accountId;
            Email = email;
            CreateDateTime = createDateTime;
        }
    }
}