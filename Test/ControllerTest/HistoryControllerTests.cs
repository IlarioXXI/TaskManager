using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManagerWEB.Api.Controllers;
using TaskManagerWEB.Api.ViewModels;
using Xunit;

namespace Test.ControllerTest
{
    public class HistoryControllerTests
    {
        private readonly HistoryController _controller;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHistoryService> _historyService;
        public HistoryControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _historyService = new Mock<IHistoryService>();
            _controller = new HistoryController(_historyService.Object,_mapperMock.Object);
        }

        [Fact]
        public void GetAll_ValidTaskId_ReturnsOkResult()
        {


            // Arrange
            int taskId = 1;
            var historyItems = new List<History>
            {
                new History { Id = 1, TaskItemId = taskId, FromStatus = "History 1",ToStatus = "ciao" },
                new History { Id = 2, TaskItemId = taskId, FromStatus = "History 2", ToStatus = "ciao" }
            };
            var historyVM = new List<HistoryVM>
            {
                new HistoryVM { Id = 1, TaskItemId = taskId, FromStatus = "History 1", ToStatus = "ciao" },
                new HistoryVM { Id = 2, TaskItemId = taskId, FromStatus = "History 2", ToStatus = "ciao" }
            };

            _historyService.Setup(h => h.GetAllByTaskId(taskId))
                .Returns(historyItems);

            _mapperMock.Setup(m => m.Map<IEnumerable<History>, IEnumerable<HistoryVM>>(It.IsAny<IEnumerable<History>>()))
                .Returns(historyVM);

            // Act
            var result = _controller.GetAll(taskId);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(historyVM, okResult.Value);
        }
    }
}
