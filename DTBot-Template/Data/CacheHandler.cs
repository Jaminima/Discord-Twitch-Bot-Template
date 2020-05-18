﻿using DTBot_Template.Generics;
using System;
using System.Collections.Generic;

namespace DTBot_Template.Data
{
    public static class CacheHandler
    {
        #region Fields

        private static Dictionary<CurrencyConfig, DateTime> _currencyCache = new Dictionary<CurrencyConfig, DateTime>();
        private static Dictionary<CurrencyParticipant, DateTime> _currencyParticipantCache = new Dictionary<CurrencyParticipant, DateTime>();
        private static List<_userInfo> _userCache = new List<_userInfo>();

        #endregion Fields

        #region Methods

        private static T Find<T>(Dictionary<T, DateTime> Cache, Func<T, bool> Pred)
        {
            foreach (T Key in Cache.Keys)
            {
                if ((DateTime.Now - Cache[Key]).TotalSeconds > 60) Cache.Remove(Key);
                else if (Pred(Key)) return Key;
            }
            return default(T);
        }

        public static _userInfo AddUser(_userInfo user)
        {
            user.Insert();
            user = _userInfo.Find(user.user, user.currency);
            _userCache.Add(user);
            return user;
        }

        public static CurrencyConfig FindCurrency(uint curid)
        {
            CurrencyConfig _currency = Find(_currencyCache, x => x.Id == curid);

            if (_currency == null)
            {
                _currency = CurrencyConfig.Find(curid);
                if (_currency != null) _currencyCache.Add(_currency, DateTime.Now);
            }

            return _currency;
        }

        public static CurrencyConfig FindCurrency(string Source, Source source)
        {
            CurrencyParticipant _participant = Find(_currencyParticipantCache, x => (x.discord_guild == Source && source == Generics.Source.Discord) || (x.twitch_name == Source && source == Generics.Source.Twitch));

            if (_participant == null)
            {
                if (source == Generics.Source.Discord) _participant = CurrencyParticipant.FindDiscord(Source);
                else _participant = CurrencyParticipant.FindTwitch(Source);

                if (_participant != null) _currencyParticipantCache.Add(_participant, DateTime.Now);
            }

            return FindCurrency(_participant.currencyid);
        }

        public static _userInfo FindUser(User user, uint curid)
        {
            _userInfo _uInfo = _userCache.Find(x => x.user.Equals(user) && x.currency == curid);

            if (_uInfo == null)
            {
                _uInfo = _userInfo.Find(user, curid);
                if (_uInfo != null) _userCache.Add(_uInfo);
            }

            if (_uInfo == null)
            {
                _uInfo = new _userInfo(user);
                _uInfo.currency = curid;
                return AddUser(_uInfo);
            }

            return _uInfo;
        }

        public static _userInfo[] FindUsers(User[] users, uint curid)
        {
            _userInfo[] _uInfos = new _userInfo[users.Length];

            for (int i = 0; i < users.Length; i++)
            {
                _uInfos[i] = FindUser(users[i], curid);
            }

            return _uInfos;
        }

        #endregion Methods
    }
}