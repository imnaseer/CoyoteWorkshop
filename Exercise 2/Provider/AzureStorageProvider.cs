using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Coyote.Tasks;

namespace TinyService
{
    public class AzureStorageProvider
    {
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>> storageAccounts =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>>();

        private Logger logger;

        public AzureStorageProvider(string context, Logger parentLogger)
        {
            this.logger = new Logger($"{nameof(AzureStorageProvider)} - {context}", parentLogger.FlowId);
        }

        public async Task CreateAccount(string accountName)
        {
            await Task.Yield();

            logger.Write($"CreateAccount {accountName}");

            if (storageAccounts.ContainsKey(accountName))
            {
                throw new AzureStorageException($"Storage account {accountName} already exists");
            }

            storageAccounts[accountName] = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        }

        public async Task DeleteAccount(string accountName)
        {
            await Task.Yield();

            logger.Write($"DeleteAccount {accountName}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                throw new AzureStorageException($"Storage account {accountName} does not exist");
            }

            ConcurrentDictionary<string, ConcurrentDictionary<string, string>> account;
            storageAccounts.Remove(accountName, out account);
        }

        public async Task<bool> DoesAccountExits(string accountName)
        {
            await Task.Yield();

            logger.Write($"DoesAccountExits {accountName}");

            return storageAccounts.ContainsKey(accountName);
        }

        public async Task CreateContainer(string accountName, string containerName)
        {
            await Task.Yield();

            logger.Write($"CreateContainer {accountName} {containerName}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                throw new AzureStorageException($"Storage account {accountName} does not exist");
            }

            if (storageAccounts[accountName].ContainsKey(containerName))
            {
                throw new AzureStorageException($"Storage container {accountName}:{containerName} already exists");
            }

            storageAccounts[accountName][containerName] = new ConcurrentDictionary<string, string>();
        }

        public async Task DeleteContainer(string accountName, string containerName)
        {
            await Task.Yield();

            logger.Write($"DeleteContainer {accountName} {containerName}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                throw new AzureStorageException($"Storage account {accountName} does not exist");
            }

            if (!storageAccounts[accountName].ContainsKey(containerName))
            {
                throw new AzureStorageException($"Storage container {accountName}:{containerName} does not exist");
            }

            ConcurrentDictionary<string, string> container;
            storageAccounts[accountName].Remove(containerName, out container);
        }

        public async Task<bool> DoesContainerExist(string accountName, string containerName)
        {
            await Task.Yield();

            logger.Write($"DoesContainerExist {accountName} {containerName}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                throw new AzureStorageException($"Storage account {accountName} does not exist");
            }

            return storageAccounts[accountName].ContainsKey(containerName);
        }

        public async Task CreateBlob(string accountName, string containerName, string blobName, string blobContents)
        {
            await Task.Yield();

            logger.Write($"CreateBlob {accountName} {containerName} {blobName} {blobContents}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                throw new AzureStorageException($"Storage account {accountName} does not exist");
            }

            if (!storageAccounts[accountName].ContainsKey(containerName))
            {
                throw new AzureStorageException($"Storage container {accountName}:{containerName} does not exist");
            }

            if (storageAccounts[accountName][containerName].ContainsKey(blobName))
            {
                throw new AzureStorageException($"Storage blob {accountName}:{containerName}:{blobName} already exists");
            }

            storageAccounts[accountName][containerName][blobName] = blobContents;
        }

        public async Task<string> GetBlobContents(string accountName, string containerName, string blobName)
        {
            await Task.Yield();

            logger.Write($"GetBlobContents {accountName} {containerName} {blobName}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                throw new AzureStorageException($"Storage account {accountName} does not exist");
            }

            if (!storageAccounts[accountName].ContainsKey(containerName))
            {
                throw new AzureStorageException($"Storage container {accountName}:{containerName} does not exist");
            }

            if (!storageAccounts[accountName][containerName].ContainsKey(blobName))
            {
                throw new AzureStorageException($"Storage blob {accountName}:{containerName}:{blobName} does not exist");
            }

            return storageAccounts[accountName][containerName][blobName];
        }

        public async Task DeleteBlob(string accountName, string containerName, string blobName)
        {
            await Task.Yield();

            logger.Write($"DeleteBlob {accountName} {containerName} {blobName}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                throw new AzureStorageException($"Storage account {accountName} does not exist");
            }

            if (!storageAccounts[accountName].ContainsKey(containerName))
            {
                throw new AzureStorageException($"Storage container {accountName}:{containerName} does not exist");
            }

            if (!storageAccounts[accountName][containerName].ContainsKey(blobName))
            {
                throw new AzureStorageException($"Storage blob {accountName}:{containerName}:{blobName} does not exist");
            }

            string blob;
            storageAccounts[accountName][containerName].Remove(blobName, out blob);
        }

        public async Task<bool> DoesBlobExist(string accountName, string containerName, string blobName)
        {
            await Task.Yield();

            logger.Write($"DoesBlobExist {accountName} {containerName} {blobName}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                throw new AzureStorageException($"Storage account {accountName} does not exist");
            }

            if (!storageAccounts[accountName].ContainsKey(containerName))
            {
                throw new AzureStorageException($"Storage container {accountName}:{containerName} does not exist");
            }

            return storageAccounts[accountName][containerName].ContainsKey(blobName);
        }

        public async Task<bool> CreateAccountIfNotExists(string accountName)
        {
            logger.Write($"CreateAccountIfNotExists {accountName}");

            try
            {
                await CreateAccount(accountName);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
        }

        public async Task<bool> CreateContainerIfNotExists(string accountName, string containerName)
        {
            logger.Write($"CreateContainerIfNotExists {accountName} {containerName}");

            try
            {
                await CreateContainer(accountName, containerName);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
        }

        public async Task<bool> CreateBlobIfNotExists(string accountName, string containerName, string blobName, string blobContents)
        {
            logger.Write($"CreateBlobIfNotExists {accountName} {containerName} {blobName} {blobContents}");

            try
            {
                await CreateBlob(accountName, containerName, blobName, blobContents);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteBlobIfExists(string accountName, string containerName, string blobName)
        {
            logger.Write($"DeleteBlobIfExists {accountName} {containerName} {blobName}");

            try
            {
                await DeleteBlob(accountName, containerName, blobName);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteContainerIfExists(string accountName, string containerName)
        {
            logger.Write($"DeleteContainerIfExists {accountName} {containerName}");

            try
            {
                await DeleteContainer(accountName, containerName);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAccountIfExists(string accountName)
        {
            logger.Write($"DeleteAccountIfExists {accountName}");

            try
            {
                await DeleteAccount(accountName);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
        }

        public async Task<string> GetBlobContentsIfExist(string accountName, string containerName, string blobName)
        {
            logger.Write($"GetBlobContentsIfExist {accountName} {containerName} {blobName}");

            try
            {
                return await GetBlobContents(accountName, containerName, blobName);
            }
            catch (AzureStorageException)
            {
                return null;
            }
        }

        public static void Cleanup()
        {
            storageAccounts = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>>();
        }
    }

    public class AzureStorageException : Exception
    {
        public AzureStorageException(string message) : base(message)
        {
        }
    }
}
