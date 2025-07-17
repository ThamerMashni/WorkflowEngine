using WorkflowEngine.Core.Entities;

namespace WorkflowEngine.Core.Models
{
    public class FlowManager : BaseEntity
    {
        public string Name { get; set; }
        public FlowManagerStatus Status { get; set; }
        public List<FlowRoute> Routes { get; set; } = new List<FlowRoute>();
        public int Order { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsActive { get; set; } = true;
        public string Type { get; set; } // Just a string, not an enum

        public FlowRoute GetCurrentRoute()
        {
            return Routes?.FirstOrDefault(r => r.IsActive && r.IsCurrent);
        }

        public void StartFlow()
        {
            if (Status != FlowManagerStatus.NotStarted) return;

            var firstRoute = Routes?.Where(r => r.IsActive).OrderBy(r => r.Order).FirstOrDefault();
            if (firstRoute != null)
            {
                IsCurrent = true;
                Status = FlowManagerStatus.InProgress;
                firstRoute.IsCurrent = true;
                firstRoute.StartTheFlow();
            }
        }
        /// <summary>
        /// Activates a route by its tag, deactivating others if exclusive
        /// </summary>
        public bool ActivateRoute(int routeTag, bool exclusive = false)
        {
            var route = Routes.FirstOrDefault(r => r.Tag == routeTag);
            if (route == null) return false;

            if (exclusive)
            {
                // Deactivate all other routes
                foreach (var r in Routes.Where(r => r.Tag != routeTag))
                {
                    r.IsActive = false;
                }
            }

            route.IsActive = true;
            return true;
        }

        /// <summary>
        /// Deactivates a route by its tag
        /// </summary>
        public bool DeactivateRoute(int routeTag)
        {
            var route = Routes.FirstOrDefault(r => r.Tag == routeTag);
            if (route == null) return false;

            route.IsActive = false;

            // If this was the current route, we need to handle the workflow state
            if (route.IsCurrent)
            {
                route.IsCurrent = false;

                // Reset any current stage in this route
                var currentStage = route.GetCurrentStage();
                if (currentStage != null)
                {
                    currentStage.IsCurrent = false;
                }

                // Update route status
                route.Status = FlowRouteStatus.Cancelled;
            }

            return true;
        }

        /// <summary>
        /// Activates multiple routes by their tags
        /// </summary>
        public int ActivateRoutes(params int[] routeTags)
        {
            int activated = 0;
            foreach (var tag in routeTags)
            {
                if (ActivateRoute(tag, false))
                    activated++;
            }
            return activated;
        }

        /// <summary>
        /// Deactivates multiple routes by their tags
        /// </summary>
        public int DeactivateRoutes(params int[] routeTags)
        {
            int deactivated = 0;
            foreach (var tag in routeTags)
            {
                if (DeactivateRoute(tag))
                    deactivated++;
            }
            return deactivated;
        }

        /// <summary>
        /// Deactivates all routes except the specified ones
        /// </summary>
        public int DeactivateAllRoutesExcept(params int[] keepActiveTags)
        {
            int deactivated = 0;
            foreach (var route in Routes.Where(r => !keepActiveTags.Contains(r.Tag)))
            {
                if (route.IsActive)
                {
                    route.IsActive = false;
                    if (route.IsCurrent)
                    {
                        route.IsCurrent = false;
                        route.Status = FlowRouteStatus.Cancelled;
                        var currentStage = route.GetCurrentStage();
                        if (currentStage != null)
                        {
                            currentStage.IsCurrent = false;
                        }
                    }
                    deactivated++;
                }
            }
            return deactivated;
        }

        /// <summary>
        /// Gets all currently active routes
        /// </summary>
        public IEnumerable<FlowRoute> GetActiveRoutes()
        {
            return Routes.Where(r => r.IsActive);
        }

        /// <summary>
        /// Gets all inactive routes
        /// </summary>
        public IEnumerable<FlowRoute> GetInactiveRoutes()
        {
            return Routes.Where(r => !r.IsActive);
        }

        /// <summary>
        /// Checks if a specific route is active
        /// </summary>
        public bool IsRouteActive(int routeTag)
        {
            var route = Routes.FirstOrDefault(r => r.Tag == routeTag);
            return route?.IsActive ?? false;
        }

        /// <summary>
        /// Switches from one route to another (deactivates current, activates new)
        /// </summary>
        public bool SwitchToRoute(int newRouteTag)
        {
            var currentRoute = GetCurrentRoute();
            var newRoute = Routes.FirstOrDefault(r => r.Tag == newRouteTag);

            if (newRoute == null) return false;

            // Deactivate current route if exists
            if (currentRoute != null)
            {
                currentRoute.IsCurrent = false;
                currentRoute.Status = FlowRouteStatus.Cancelled;

                var currentStage = currentRoute.GetCurrentStage();
                if (currentStage != null)
                {
                    currentStage.IsCurrent = false;
                }
            }

            // Activate and set as current the new route
            newRoute.IsActive = true;
            newRoute.IsCurrent = true;
            newRoute.StartTheFlow();

            return true;
        }
    }
}