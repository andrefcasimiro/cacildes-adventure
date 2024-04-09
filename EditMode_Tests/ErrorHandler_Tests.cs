using NUnit.Framework;
using UnityEngine;

namespace AF.Tests
{
    public class ErrorHandlerTests
    {
        ErrorHandler errorHandler;

        [SetUp]
        public void SetUp()
        {
            errorHandler = new GameObject().AddComponent<ErrorHandler>();
        }

        [Test]
        public void DisplayErrorPanel_AddsErrorEntry_WhenNotIgnored()
        {
            var errorMessage = "Test Error";
            var stackTrace = "Test Stack Trace";

            // Act
            errorHandler.DisplayErrorPanel(errorMessage, stackTrace);

            // Assert
            Assert.That(errorHandler.errors.Count == 1);
        }

        [Test]
        public void DisplayErrorPanel_DoesNotAddErrorEntry_WhenIgnored()
        {
            // Arrange
            var errorMessage = "PhysX does not support concave Mesh Colliders with dynamic Rigidbody GameObjects.";
            var stackTrace = "Test Stack Trace";

            // Act
            errorHandler.DisplayErrorPanel(errorMessage, stackTrace);

            // Assert
            Assert.That(errorHandler.HasErrors(), Is.False);
        }

        [Test]
        public void CopyToClipboard_CopiesErrorMessageAndStackTrace()
        {
            // Arrange
            var errorMessage = "Test Error";
            var stackTrace = "Test Stack Trace";

            // Act
            errorHandler.CopyToClipboard(errorMessage, stackTrace);

            // Assert
            Assert.AreEqual("Error Name: Test Error\n Stack Trace: Test Stack Trace", GUIUtility.systemCopyBuffer);
        }

    }
}
