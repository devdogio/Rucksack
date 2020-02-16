using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Devdog.Rucksack.Collections
{
    public enum CollectionCreatorRegisterBy { Both, Name, ID }
    public abstract class BaseCollectionCreator<TCollection> : MonoBehaviour
    {
        protected Devdog.Rucksack.ILogger _logger;

        [Header("Startup Settings"), SerializeField]
        private bool _initOnStart = true;

        [Header("Collection Settings"), SerializeField]
        private string _collectionName;
        [SerializeField]
        private bool _useUniqueName = false;
        [SerializeField]
        private bool _ignoreDuplicates = false;
        [SerializeField]
        private SerializedGuid _guid;
        [SerializeField]
        private bool _generateIDOnStart = false;
        [SerializeField]
        private CollectionCreatorRegisterBy _registerBy = CollectionCreatorRegisterBy.Both;

        /// <summary>
        /// Check if creator should be initialized on start
        /// </summary>
        public bool initOnStart => _initOnStart;

        /// <summary>
        /// Name of the collection to be created
        /// </summary>
        public string collectionName
        {
            get { return _collectionName; }
            set { _collectionName = value; }
        }

        /// <summary>
        /// Id of the collection to be created
        /// </summary>
        public Guid collectionID
        {
            get { return _guid.guid; }
            set { _guid.guid = value; }
        }

        /// <summary>
        /// Check if registry should ignore name duplicates when register collection by its name
        /// </summary>
        public bool ignoreDuplicates
        {
            get { return _ignoreDuplicates; }
            set { _ignoreDuplicates = value; }
        }

        /// <summary>
        /// Check if creator should generate <see cref="collectionID"/> at start
        /// </summary>
        public bool generateIDOnStart
        {
            get { return _generateIDOnStart; }
            set { _generateIDOnStart = value; }
        }

        /// <summary>
        /// Choose collection registration method
        /// </summary>
        public CollectionCreatorRegisterBy registerBy
        {
            get { return _registerBy; }
            set { _registerBy = value; }
        }

        /// <summary>
        /// Check if creator should create unique collection name by concatenation of <see cref="collectionName"/> and <see cref="collectionID"/>
        /// </summary>
        public bool useUniqueName
        {
            get { return _useUniqueName; }
            set { _useUniqueName = value; }
        }

        /// <summary>
        /// Created collection
        /// </summary>
        public TCollection collection { get; protected set; }

        protected virtual void Awake()
        {
            if (initOnStart)
            {
                Initialize();
            }
        }

        public virtual void Initialize()
        {
            if (_generateIDOnStart)
            {
                _guid.guid = Guid.NewGuid();
            }

            _logger = new UnityLogger($"[{name}]");

            collectionName = useUniqueName ? collectionName + "_" + collectionID.ToString("N") : collectionName;
            collection = CreateCollection();

            switch (_registerBy)
            {
                case CollectionCreatorRegisterBy.Name:
                    RegisterByName(collection);
                    break;
                case CollectionCreatorRegisterBy.ID:
                    RegiterByID(collection);
                    break;
                default:
                    RegisterByName(collection);
                    RegiterByID(collection);
                    break;
            }

            _logger.LogVerbose($"Created and registered collection with name {collectionName} and guid {collectionID}", this);
        }

        protected virtual void OnDestroy()
        {
            UnRegister();
        }

        protected abstract TCollection CreateCollection();

        protected abstract void RegisterByName(TCollection col);

        protected abstract void RegiterByID(TCollection col);

        protected abstract void UnRegister();
    }
}