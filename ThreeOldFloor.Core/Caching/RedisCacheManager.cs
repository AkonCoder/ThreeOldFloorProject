using System;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ThreeOldFloor.Core.Caching
{
  
    public partial class RedisCacheManager : ICacheManager
    {
        #region Fields

        private readonly ConnectionMultiplexer _muxer;
        private readonly IDatabase _db;
        #endregion

        #region Ctor

        public RedisCacheManager(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
                throw  new Exception("Redis connection string is empty");

            this._muxer = ConnectionMultiplexer.Connect(connectionString);

            this._db = _muxer.GetDatabase();
          
        }

        #endregion

        #region Utilities

        protected virtual byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(jsonString);
        }
        protected virtual T Deserialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null)
                return default(T);

            var jsonString = Encoding.UTF8.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        #endregion

        #region Methods

    
        public virtual T Get<T>(string key)
        {
            
            var rValue = _db.StringGet(key);
            if (!rValue.HasValue)
                return default(T);
            var result = Deserialize<T>(rValue);

            return result;
        }

    
        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            var entryBytes = Serialize(data);
            var expiresIn = TimeSpan.FromMinutes(cacheTime);

            _db.StringSet(key, entryBytes, expiresIn);
        }

    
        public virtual bool IsSet(string key)
        {
         
            return _db.KeyExists(key);
        }

       
        public virtual void Remove(string key)
        {
            _db.KeyDelete(key);
        }

     
        public virtual void RemoveByPattern(string pattern)
        {
            foreach (var ep in _muxer.GetEndPoints())
            {
                var server = _muxer.GetServer(ep);
                var keys = server.Keys(pattern: "*" + pattern + "*");
                foreach (var key in keys)
                    _db.KeyDelete(key);
            }
        }

        public virtual void Clear()
        {
            foreach (var ep in _muxer.GetEndPoints())
            {
                var server = _muxer.GetServer(ep);
             
                var keys = server.Keys();
                foreach (var key in keys)
                    _db.KeyDelete(key);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            if (_muxer != null)
                _muxer.Dispose();
        }

        #endregion
    }
}