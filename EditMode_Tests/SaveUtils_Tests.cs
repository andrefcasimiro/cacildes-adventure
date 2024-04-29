using UnityEngine;
using NUnit.Framework;
using System.IO;

namespace AF.Tests
{
    public class SaveUtilsTests
    {
        [Test]
        public void HasSaveFiles_DirectoryExistsWithFiles_ReturnsTrue()
        {
            // Arrange
            string testFolderPath = Path.Combine(Application.persistentDataPath, "QuickSave_TMP");
            Directory.CreateDirectory(testFolderPath);
            File.WriteAllText(Path.Combine(testFolderPath, "testfile.sav"), "Test data");

            // Act
            bool result = SaveUtils.HasSaveFiles("QuickSave_TMP");

            // Assert
            Assert.IsTrue(result);

            // Clean up
            Directory.Delete(testFolderPath, true);
        }

        [Test]
        public void HasSaveFiles_DirectoryDoesNotExist_ReturnsFalse()
        {
            // Act
            bool result = SaveUtils.HasSaveFiles("QuickSave_TMP");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void HasSaveFiles_DirectoryExistsButNoFiles_ReturnsFalse()
        {
            // Arrange
            string testFolderPath = Path.Combine(Application.persistentDataPath, "QuickSave_TMP");
            Directory.CreateDirectory(testFolderPath);

            // Act
            bool result = SaveUtils.HasSaveFiles("QuickSave_TMP");

            // Assert
            Assert.IsFalse(result);

            // Clean up
            Directory.Delete(testFolderPath);
        }
    }
}
