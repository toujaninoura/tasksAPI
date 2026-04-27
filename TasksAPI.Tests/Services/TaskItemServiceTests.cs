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
    public async Task GetByIdAsync_should_return_task_with_all_fields()
    {
        var task = new TaskItem("Tache detail", "in-progress") { Id = 42 };
        _repositoryMock.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(task);

        var result = await _service.GetByIdAsync(42);

        result.Should().NotBeNull();
        result!.Id.Should().Be(42);
        result.Titre.Should().Be("Tache detail");
        result.Statut.Should().Be("in-progress");
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

    // Issue #4 - POST /tasks : statut par defaut "todo" si non fourni
    [Test]
    public async Task CreateAsync_should_default_statut_to_todo_when_not_provided()
    {
        var request = new CreateTaskItemRequest { Titre = "Tache sans statut" };
        var task = new TaskItem("Tache sans statut", "todo") { Id = 5 };
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).ReturnsAsync(task);

        var result = await _service.CreateAsync(request);

        result.Statut.Should().Be("todo");
    }

    // Issue #4 - POST /tasks : retourne la tache creee avec id, titre, statut
    [Test]
    public async Task CreateAsync_should_return_created_task_with_all_fields()
    {
        var request = new CreateTaskItemRequest { Titre = "Ma tache", Statut = "in-progress" };
        var task = new TaskItem("Ma tache", "in-progress") { Id = 10 };
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).ReturnsAsync(task);

        var result = await _service.CreateAsync(request);

        result.Id.Should().Be(10);
        result.Titre.Should().Be("Ma tache");
        result.Statut.Should().Be("in-progress");
    }

    // Issue #2 - GET /tasks : chaque tache doit contenir id, titre, statut
    [Test]
    public async Task GetAllAsync_should_return_tasks_with_id_titre_statut_fields()
    {
        var tasks = new List<TaskItem>
        {
            new TaskItem("Tache A", "todo"),
            new TaskItem("Tache B", "done")
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

        var result = await _service.GetAllAsync(1, 10);

        result.Items.Should().NotBeEmpty();
        foreach (var item in result.Items)
        {
            item.Titre.Should().NotBeNullOrEmpty();
            item.Statut.Should().NotBeNullOrEmpty();
        }
        result.Items.Should().Contain(t => t.Titre == "Tache A" && t.Statut == "todo");
        result.Items.Should().Contain(t => t.Titre == "Tache B" && t.Statut == "done");
    }

    // Issue #2 - GET /tasks : retourne 200 (ApiResponse wrapping) avec la liste
    [Test]
    public async Task GetAllAsync_should_return_paged_result_with_correct_pagination_metadata()
    {
        var tasks = Enumerable.Range(1, 15)
            .Select(i => new TaskItem($"Tache {i}", "todo"))
            .ToList();
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

        var result = await _service.GetAllAsync(2, 5);

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalCount.Should().Be(15);
        result.Items.Should().HaveCount(5);
    }
}
