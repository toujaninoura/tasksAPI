using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TasksAPI.Application.DTOs;
using TasksAPI.Application.Interfaces;
using TasksAPI.Application.Mappings;
using TasksAPI.Application.Services;
using TasksAPI.Domain.Entities;
using TasksAPI.Domain.Exceptions;

namespace TasksAPI.Tests.Services;

[TestFixture]
public class TaskItemServiceTests
{
    private Mock<ITaskItemRepository> _repositoryMock = null!;
    private IMapper _mapper = null!;
    private TaskItemService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new TaskItemProfile()));
        _mapper = config.CreateMapper();
        _service = new TaskItemService(_repositoryMock.Object, _mapper);
    }

    [Test]
    public async Task GetAllAsync_should_return_paged_result_when_tasks_exist()
    {
        var tasks = new List<TaskItem>
        {
            new TaskItem("Tache 1", "todo"),
            new TaskItem("Tache 2", "in-progress")
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

        var result = await _service.GetAllAsync(1, 10);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
    }

    [Test]
    public async Task GetByIdAsync_should_return_task_when_valid_id()
    {
        var task = new TaskItem("Tache test", "todo");
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Titre.Should().Be("Tache test");
        result.Statut.Should().Be("todo");
    }

    [Test]
    public async Task GetByIdAsync_should_throw_not_found_when_invalid_id()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        Func<Task> act = async () => await _service.GetByIdAsync(999);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*999*");
    }

    [Test]
    public async Task CreateAsync_should_return_created_task_when_valid_request()
    {
        var request = new CreateTaskItemRequest { Titre = "Nouvelle tache", Statut = "todo" };
        var task = new TaskItem(request.Titre, request.Statut);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).ReturnsAsync(task);

        var result = await _service.CreateAsync(request);

        result.Should().NotBeNull();
        result.Titre.Should().Be("Nouvelle tache");
        result.Statut.Should().Be("todo");
    }

    [Test]
    public async Task UpdateAsync_should_throw_not_found_when_task_does_not_exist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);
        var request = new UpdateTaskItemRequest { Titre = "Updated", Statut = "done" };

        Func<Task> act = async () => await _service.UpdateAsync(999, request);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*999*");
    }

    [Test]
    public async Task DeleteAsync_should_throw_not_found_when_task_does_not_exist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        Func<Task> act = async () => await _service.DeleteAsync(999);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*999*");
    }

    [Test]
    public async Task GetAllAsync_should_return_empty_paged_result_when_no_tasks()
    {
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TaskItem>());

        var result = await _service.GetAllAsync(1, 10);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
