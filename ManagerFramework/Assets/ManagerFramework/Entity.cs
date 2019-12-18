/*
* TRIFLES GAMES
* www.triflesgames.com
*
* Developed by Gökhan KINAY.
* www.gokhankinay.com.tr
*
* Contact,
* info@triflesgames.com
* info@gokhankinay.com.tr
*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ManagerActorFramework
{
    public abstract class Entity<TEntity> where TEntity : Entity<TEntity>
    {
        public static Regex IdRegex;

        private static Dictionary<string, TEntity> _entities;

        private TEntity _derived;

        public static string[] Ids
        {
            get;
            private set;
        }

        public string Id
        {
            get;
            private set;
        }

        static Entity()
        {
            IdRegex = new Regex("^[A-Z0-9]{4,32}$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
            _entities = new Dictionary<string, TEntity>();
            Ids = new string[0];
        }

        protected Entity()
        {
            _derived = (this as TEntity);
            if (_derived == null)
            {
                throw new ArgumentException("Derived class type mismatch!", "TEntity");
            }
            Id = string.Empty;
            if (!Init())
            {
                throw new InvalidOperationException("Cannot initialize entity!");
            }
        }

        public static TEntity Get(string id = "default")
        {
            return (!_entities.ContainsKey(id)) ? ((TEntity)null) : _entities[id];
        }

        public bool Register(string id = "default")
        {
            if (!string.IsNullOrEmpty(Id))
            {
                return false;
            }
            if (!IdRegex.IsMatch(id))
            {
                return false;
            }
            if (_entities.ContainsKey(id))
            {
                return false;
            }
            _entities.Add(id, _derived);
            Id = id;
            Ids = new string[_entities.Count];
            _entities.Keys.CopyTo(Ids, 0);
            return true;
        }

        public void Unregister()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                if (_entities.Remove(Id))
                {
                    Ids = new string[_entities.Count];
                    _entities.Keys.CopyTo(Ids, 0);
                }
                Id = string.Empty;
            }
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                string text = $"__ENTITY({typeof(TEntity).Name})#{Id}";
                string text2 = JsonUtility.ToJson((object)_derived);
                if (!string.IsNullOrEmpty(text2))
                {
                    PlayerPrefs.SetString(text, text2);
                }
            }
        }

        public void Load()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                string text = $"__ENTITY({typeof(TEntity).Name})#{Id}";
                string @string = PlayerPrefs.GetString(text);
                if (!string.IsNullOrEmpty(@string))
                {
                    JsonUtility.FromJsonOverwrite(@string, (object)_derived);
                }
            }
        }

        protected abstract bool Init();
    }
}