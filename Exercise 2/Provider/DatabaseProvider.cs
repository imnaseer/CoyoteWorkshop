using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Coyote.Tasks;

namespace TinyService
{
    using Document = ConcurrentDictionary<string, string>;
    using Collection = ConcurrentDictionary<string, ConcurrentDictionary<string, string>>;

    public class DatabaseProvider
    {
        public static ConcurrentDictionary<string, Collection> collections = new ConcurrentDictionary<string, Collection>();
        
        static DatabaseProvider()
        {
            collections[Constants.UserCollection] = new Collection();
        }

        private Logger logger;

        public DatabaseProvider(string context, Logger parentLogger)
        {
            logger = new Logger($"{nameof(DatabaseProvider)} - {context}", parentLogger.FlowId);
        }

        public async Task AddDocument(string collectionName, string rowKey, Document doc)
        {
            await Task.Run(() =>
            {
                logger.Write($"AddDocument {rowKey}, {DocToStr(doc)} in collection {collectionName}");

                if (!collections.ContainsKey(collectionName))
                {
                    throw new DatabaseException($"Collection {collectionName} does not exist");
                }

                var collection = collections[collectionName];

                if (collection.ContainsKey(rowKey))
                {
                    throw new DatabaseException($"AddRow: {rowKey} already exists");
                }
                else
                {
                    collection[rowKey] = doc;
                }
            });
        }

        public async Task UpdateDocument(string collectionName, string rowKey, Document doc)
        {
            await Task.Run(() =>
            {
                logger.Write($"UpdateDocument {rowKey}, {DocToStr(doc)} in collection {collectionName}");

                if (!collections.ContainsKey(collectionName))
                {
                    throw new DatabaseException($"Collection {collectionName} does not exist");
                }

                var collection = collections[collectionName];

                if (!collection.ContainsKey(rowKey))
                {
                    throw new DatabaseException($"UpdateRow: {rowKey} does not exist");
                }

                collection[rowKey] = doc;
            });
        }

        public async Task UpdateKeyInDocument(string collectionName, string rowKey, string docKey, string docValue)
        {
            await Task.Run(() =>
            {
                logger.Write($"UpdateKeyInDocument for row {rowKey}, {docKey}: {docValue} in collection {collectionName}");

                if (!collections.ContainsKey(collectionName))
                {
                    throw new DatabaseException($"Collection {collectionName} does not exist");
                }

                var collection = collections[collectionName];

                if (!collection.ContainsKey(rowKey))
                {
                    throw new DatabaseException($"UpdateKeyInDocument: {rowKey} does not exist");
                }

                var doc = collection[rowKey];

                if (!doc.ContainsKey(docKey))
                {
                    throw new DatabaseException($"UpdateKeyInDocument: {docKey} does not exist in document");
                }

                doc[docKey] = docValue;
            });
        }

        public async Task DeleteDocument(string collectionName, string rowKey)
        {
            await Task.Run(() =>
            {
                logger.Write($"DeleteDocument {rowKey} in collection {collectionName}");

                if (!collections.ContainsKey(collectionName))
                {
                    throw new DatabaseException($"Collection {collectionName} does not exist");
                }

                var collection = collections[collectionName];

                if (!collection.ContainsKey(rowKey))
                {
                    throw new DatabaseException($"DeleteRow: {rowKey} does not exist");
                }
                else
                {
                    Document doc;
                    collection.Remove(rowKey, out doc);
                }
            });
        }

        public async Task<Document> GetDocument(string collectionName, string rowKey)
        {
            return await Task.Run(() =>
            {
                logger.Write($"GetDocument {rowKey} in collection {collectionName}");

                if (!collections.ContainsKey(collectionName))
                {
                    throw new DatabaseException($"Collection {collectionName} does not exist");
                }

                var collection = collections[collectionName];

                if (!collection.ContainsKey(rowKey))
                {
                    throw new DatabaseException($"GetValue: {rowKey} does not exit");
                }

                return collection[rowKey];
            });
        }

        public async Task<bool> DoesDocumentExist(string collectionName, string rowKey)
        {
            return await Task.Run(() =>
            {
                logger.Write($"DoesDocumentExist {rowKey} in collection {collectionName}");

                if (!collections.ContainsKey(collectionName))
                {
                    throw new DatabaseException($"Collection {collectionName} does not exist");
                }

                var table = collections[collectionName];

                return table.ContainsKey(rowKey);
            });
        }

        public async Task<bool> AddDocumentIfNotExists(string collectionName, string rowKey, Document doc)
        {
            logger.Write($"AddDocumentIfNotExists {rowKey}, {DocToStr(doc)} in collection {collectionName}");

            try
            {
                await this.AddDocument(collectionName, rowKey, doc);
                return true;
            }
            catch (DatabaseException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateDocumentIfExists(string collectionName, string rowKey, Document doc)
        {
            logger.Write($"UpdateDocumentIfExists {rowKey}, {DocToStr(doc)} in collection {collectionName}");

            try
            {
                await this.UpdateDocument(collectionName, rowKey, doc);
                return true;
            }
            catch (DatabaseException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateKeyInDocumentIfExists(string collectionName, string rowKey, string docKey, string docValue)
        {
            logger.Write($"UpdateKeyInDocumentIfExists {rowKey}, {docKey}, {docValue} in collection {collectionName}");

            try
            {
                await this.UpdateKeyInDocument(collectionName, rowKey, docKey, docValue);
                return true;
            }
            catch (DatabaseException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteDocumentIfExists(string collectionName, string rowKey)
        {
            logger.Write($"DeleteDocumentIfExists {rowKey} in collection {collectionName}");

            try
            {
                await this.DeleteDocument(collectionName, rowKey);
                return true;
            }
            catch (DatabaseException)
            {
                return false;
            }
        }

        public async Task<Document> GetDocumentIfExists(string collectionName, string rowKey)
        {
            logger.Write($"GetDocumentIfExists {rowKey} in collection {collectionName}");

            try
            {
                var value = await GetDocument(collectionName, rowKey);
                return value;
            }
            catch (DatabaseException)
            {
                return null;
            }
        }

        public static void Cleanup()
        {
            collections[Constants.UserCollection].Clear();
        }

        private static string DocToStr(Document doc)
        {
            List<string> kvPairs = new List<string>();
            foreach (var key in doc.Keys)
            {
                kvPairs.Add($"{key}: {doc[key]}");
            }
            return "{ " + string.Join(", ", kvPairs) + " }";
        }
    }

    public class DatabaseException : Exception
    {
        public DatabaseException(string msg) : base(msg)
        {
        }
    }
}
