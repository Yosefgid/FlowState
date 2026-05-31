using FlowState.Models;
using Microsoft.AspNetCore.Components;

namespace FlowState.Blazer.Components.Functionality
{
    public enum Quadrant { DoFirst, Schedule, Delegate, Eliminate }

    public abstract class BaseEisenhowerMatrix : ComponentBase
    {
        // ── Parameters ──────────────────────────────────────────────────────────

        /// <summary>
        /// Feed in your existing ToDo list from a parent page or the Dashboard.
        /// Tasks are distributed across quadrants on first render.
        /// </summary>

        [Inject]
        protected TaskStateService TaskState { get; set; } = default!;

        public List<ToDoTask> Tasks { get; set; } = new();

        // ── State ────────────────────────────────────────────────────────────────

        // Maps every task ID to its current quadrant
        protected Dictionary<int, Quadrant> taskQuadrants = new();

        // Drag state
        protected ToDoTask? draggingTask;
        protected Quadrant? dragOverQuadrant;

        // ── Lifecycle ────────────────────────────────────────────────────────────

        protected override async Task OnInitializedAsync()
        {
            // Assign any new tasks that don't yet have a quadrant.
            // Default: DoFirst — caller can pre-assign via OnTaskQuadrantChanged.
           
            await TaskState.RefreshTasks();

            Tasks = TaskState.Tasks;
            foreach (var task in Tasks)
            {
                if (!taskQuadrants.ContainsKey(task.Id))
                    taskQuadrants[task.Id] = Quadrant.DoFirst;
            }

            // Remove orphaned entries (tasks removed from the parent list)
            var activeIds = Tasks.Select(t => t.Id).ToHashSet();
            foreach (var id in taskQuadrants.Keys.Where(k => !activeIds.Contains(k)).ToList())
                taskQuadrants.Remove(id);
        }

        // ── Computed helpers ─────────────────────────────────────────────────────

        protected List<ToDoTask> AllTasks => Tasks;

        protected List<ToDoTask> TasksFor(Quadrant q) =>
            Tasks.Where(t => taskQuadrants.TryGetValue(t.Id, out var tq) && tq == q).ToList();

        // ── Drag & drop ──────────────────────────────────────────────────────────

        protected void OnDragStart(ToDoTask task)
        {
            draggingTask = task;
        }

        protected void OnDragEnd()
        {
            draggingTask = null;
            dragOverQuadrant = null;
        }

        protected void OnDrop(Quadrant target)
        {
            if (draggingTask is null) return;

            taskQuadrants[draggingTask.Id] = target;
            draggingTask = null;
            dragOverQuadrant = null;

            // Notify parent if wired up
            OnTaskMoved.InvokeAsync((draggingTask?.Id ?? 0, target));
        }

        // ── Task toggle ──────────────────────────────────────────────────────────

        protected void ToggleTask(ToDoTask task)
        {
            task.IsCompleted = !task.IsCompleted;
        }

        // ── Callbacks ────────────────────────────────────────────────────────────

        /// <summary>
        /// Fires when a task is moved to a new quadrant.
        /// Payload: (taskId, newQuadrant)
        /// </summary>
        [Parameter]
        public EventCallback<(int TaskId, Quadrant NewQuadrant)> OnTaskMoved { get; set; }
    }
}
