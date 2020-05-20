using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                var message = $"Storage account {accountName} already exists";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            storageAccounts[accountName] = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        }

        public async Task DeleteAccount(string accountName)
        {
            await Task.Yield();

            logger.Write($"DeleteAccount {accountName}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                var message = $"Storage account {accountName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
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
                var message = $"Storage account {accountName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            if (storageAccounts[accountName].ContainsKey(containerName))
            {
                var message = $"Storage container {accountName}:{containerName} already exists";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            storageAccounts[accountName][containerName] = new ConcurrentDictionary<string, string>();
        }

        public async Task DeleteContainer(string accountName, string containerName)
        {
            await Task.Yield();

            logger.Write($"DeleteContainer {accountName} {containerName}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                var message = $"Storage account {accountName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            if (!storageAccounts[accountName].ContainsKey(containerName))
            {
                var message = $"Storage container {accountName}:{containerName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
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
                var message = $"Storage account {accountName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            return storageAccounts[accountName].ContainsKey(containerName);
        }

        public async Task CreateBlob(string accountName, string containerName, string blobName, string blobContents)
        {
            await Task.Yield();

            logger.Write($"CreateBlob {accountName} {containerName} {blobName} {blobContents}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                var message = $"Storage account {accountName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            if (!storageAccounts[accountName].ContainsKey(containerName))
            {
                var message = $"Storage container {accountName}:{containerName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            if (storageAccounts[accountName][containerName].ContainsKey(blobName))
            {
                var message = $"Storage blob {accountName}:{containerName}:{blobName} already exists";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            storageAccounts[accountName][containerName][blobName] = blobContents;
        }

        public async Task<string> GetBlobContents(string accountName, string containerName, string blobName)
        {
            await Task.Yield();

            logger.Write($"GetBlobContents {accountName} {containerName} {blobName}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                var message = $"Storage account {accountName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            if (!storageAccounts[accountName].ContainsKey(containerName))
            {
                var message = $"Storage container {accountName}:{containerName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            if (!storageAccounts[accountName][containerName].ContainsKey(blobName))
            {
                var message = $"Storage blob {accountName}:{containerName}:{blobName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            return storageAccounts[accountName][containerName][blobName];
        }

        public async Task DeleteBlob(string accountName, string containerName, string blobName)
        {
            await Task.Yield();

            logger.Write($"DeleteBlob {accountName} {containerName} {blobName}");

            if (!storageAccounts.ContainsKey(accountName))
            {
                var message = $"Storage account {accountName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            if (!storageAccounts[accountName].ContainsKey(containerName))
            {
                var message = $"Storage container {accountName}:{containerName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            if (!storageAccounts[accountName][containerName].ContainsKey(blobName))
            {
                var message = $"Storage blob {accountName}:{containerName}:{blobName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
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
                var message = $"Storage account {accountName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            if (!storageAccounts[accountName].ContainsKey(containerName))
            {
                var message = $"Storage container {accountName}:{containerName} does not exist";
                logger.WriteException(message);
                throw new AzureStorageException(message);
            }

            return storageAccounts[accountName][containerName].ContainsKey(blobName);
        }

        public async Task<bool> CreateAccountIfNotExists(string accountName)
        {
            logger.Write($"CreateAccountIfNotExists {accountName}");

            try
            {
                logger.SuppressExceptionLogging(true);
                await CreateAccount(accountName);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
            finally
            {
                logger.SuppressExceptionLogging(false);
            }
        }

        public async Task<bool> CreateContainerIfNotExists(string accountName, string containerName)
        {
            logger.Write($"CreateContainerIfNotExists {accountName} {containerName}");

            try
            {
                logger.SuppressExceptionLogging(true);
                await CreateContainer(accountName, containerName);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
            finally
            {
                logger.SuppressExceptionLogging(false);
            }
        }

        public async Task<bool> CreateBlobIfNotExists(string accountName, string containerName, string blobName, string blobContents)
        {
            logger.Write($"CreateBlobIfNotExists {accountName} {containerName} {blobName} {blobContents}");

            try
            {
                logger.SuppressExceptionLogging(true);
                await CreateBlob(accountName, containerName, blobName, blobContents);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
            finally
            {
                logger.SuppressExceptionLogging(false);
            }
        }

        public async Task<bool> DeleteBlobIfExists(string accountName, string containerName, string blobName)
        {
            logger.Write($"DeleteBlobIfExists {accountName} {containerName} {blobName}");

            try
            {
                logger.SuppressExceptionLogging(true);
                await DeleteBlob(accountName, containerName, blobName);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
            finally
            {
                logger.SuppressExceptionLogging(false);
            }
        }

        public async Task<bool> DeleteContainerIfExists(string accountName, string containerName)
        {
            logger.Write($"DeleteContainerIfExists {accountName} {containerName}");

            try
            {
                logger.SuppressExceptionLogging(true);
                await DeleteContainer(accountName, containerName);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
            finally
            {
                logger.SuppressExceptionLogging(false);
            }
        }

        public async Task<bool> DeleteAccountIfExists(string accountName)
        {
            logger.Write($"DeleteAccountIfExists {accountName}");

            try
            {
                logger.SuppressExceptionLogging(true);
                await DeleteAccount(accountName);
                return true;
            }
            catch (AzureStorageException)
            {
                return false;
            }
            finally
            {
                logger.SuppressExceptionLogging(false);
            }
        }

        public async Task<string> GetBlobContentsIfExist(string accountName, string containerName, string blobName)
        {
            logger.Write($"GetBlobContentsIfExist {accountName} {containerName} {blobName}");

            try
            {
                logger.SuppressExceptionLogging(true);
                return await GetBlobContents(accountName, containerName, blobName);
            }
            catch (AzureStorageException)
            {
                return null;
            }
            finally
            {
                logger.SuppressExceptionLogging(false);
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
