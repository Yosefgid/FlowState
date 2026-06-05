using FlowState.Controllers;
using FlowState.Models;
using FlowState.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

namespace FlowState.Tests.Controllers
{
    [TestFixture]
    public class ToDoTaskController_DatesTests
    {
        private Mock<IToDoTaskService> _service;
        private ToDoTaskController _controller;
        private const int UserId = 7;

        [SetUp]
        public void SetUp()
        {
            _service = new Mock<IToDoTaskService>();
            _controller = new ToDoTaskController(_service.Object);

            var identity = new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, UserId.ToString()) }, "Test");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Test]
        public void AddTask_StampsStartDateToNow_IgnoringClientValue()
        {
            var clientSent = new ToDoTask(UserId, "Task", "desc", null)
            {
                StartDate = new DateTime(1999, 1, 1) // client lies; must be ignored
            };

            ToDoTask captured = null;
            _service.Setup(s => s.AddTask(It.IsAny<ToDoTask>()))
                    .Callback<ToDoTask>(t => captured = t)
                    .Returns((ToDoTask t) => { t.Id = 1; return t; });

            var before = DateTime.Now;
            _controller.AddTask(clientSent);
            var after = DateTime.Now;

            Assert.That(captured.StartDate,
                Is.GreaterThanOrEqualTo(before).And.LessThanOrEqualTo(after));
        }

        [Test]
        public void UpdateTask_PreservesExistingStartDate()
        {
            var existing = new ToDoTask(UserId, "Task", "desc", null)
            { Id = 1, StartDate = new DateTime(2020, 5, 1) };

            var clientUpdate = new ToDoTask(UserId, "Renamed", "desc", null)
            { Id = 1, StartDate = new DateTime(1999, 1, 1) };

            _service.Setup(s => s.GetTask(1)).Returns(existing);

            ToDoTask captured = null;
            _service.Setup(s => s.UpdateTask(1, It.IsAny<ToDoTask>()))
                    .Callback<int, ToDoTask>((_, t) => captured = t)
                    .Returns((int _, ToDoTask t) => t);

            _controller.UpdateTask(1, clientUpdate);

            Assert.That(captured.StartDate, Is.EqualTo(new DateTime(2020, 5, 1)));
        }

        [Test] // regression guard: EndDate must stay editable, NOT preserved
        public void UpdateTask_AllowsEndDateToChange()
        {
            var existing = new ToDoTask(UserId, "Task", "desc", null)
            { Id = 1, EndDate = new DateTime(2020, 5, 1) };

            var clientUpdate = new ToDoTask(UserId, "Task", "desc", null)
            { Id = 1, EndDate = new DateTime(2025, 12, 31) };

            _service.Setup(s => s.GetTask(1)).Returns(existing);

            ToDoTask captured = null;
            _service.Setup(s => s.UpdateTask(1, It.IsAny<ToDoTask>()))
                    .Callback<int, ToDoTask>((_, t) => captured = t)
                    .Returns((int _, ToDoTask t) => t);

            _controller.UpdateTask(1, clientUpdate);

            Assert.That(captured.EndDate, Is.EqualTo(new DateTime(2025, 12, 31)));
        }
    }
}
