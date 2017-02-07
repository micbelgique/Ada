using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using MartineobotIOTMvvm.Models.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace MartineobotIOTMvvm.ViewModels
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        private readonly Task _emptyTask = new Task(() => { });
        private int _loadingCounter;
        private List<CancellationTokenSource> _cancellationTokenSources;
        
        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of this ViewModel 
        /// for the Design Mode and the Production Mode.
        /// </summary>
        protected ViewModelBase()
            : this(ServiceLocator.Current.GetInstance<IDataService>(), 
                   ServiceLocator.Current.GetInstance<IDialogService>(), 
                   ServiceLocator.Current.GetInstance<INavigationService>())
        {
            // ReSharper disable once VirtualMemberCallInContructor
            if (IsInDesignModeStatic) OnLoadedAsync();
        }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase,
        /// called by empty constructor (Design and Production),
        /// and for Testing also.
        /// </summary>
        /// <param name="dataservice"></param>
        /// <param name="dialogService"></param>
        /// <param name="navigationService"></param>
        protected ViewModelBase(IDataService dataservice, IDialogService dialogService, INavigationService navigationService)
        {
            DataService = dataservice;
            DialogService = dialogService;
            NavigationService = navigationService;
        }

        #endregion

        #region SERVICES

        /// <summary>
        /// Gets a reference to <see cref="IDataService" />
        /// </summary>
        protected IDataService DataService { get; private set; }

        /// <summary>
        /// Gets a reference to <see cref="IDialogService" />
        /// </summary>
        protected IDialogService DialogService { get; private set; }

        /// <summary>
        /// Gets a reference to <see cref="INavigationService" />
        /// </summary>
        protected INavigationService NavigationService { get; private set; }

        /// <summary>
        /// Registering a recipient for the forPageKey only,
        /// and to execute the action specified
        /// </summary>
        /// <typeparam name="T">Type of action values</typeparam>
        public void NavigationRegistering<T>()
        {
            // Registering to the Messenger
            Messenger.Default.Register<T>
            (
                 this,
                 async message =>
                 {
                     await OnNavigationFrom(message);
                     Messenger.Default.Unregister<T>(this);
                 }
            );
        }

        /// <param name="parameter"></param>
        protected virtual Task OnNavigationFrom(object parameter)
        {
            // Empty
            return _emptyTask;
        }
        #endregion

        #region LIFETIME and ACTIONS

        /// <summary>
        /// Gets or sets a value indicating whether the view model is currently loading. 
        /// </summary>
        public bool IsLoading
        {
            get { return _loadingCounter > 0; }
            set
            {
                if (value)
                    _loadingCounter++;
                else if (_loadingCounter > 0)
                    _loadingCounter--;

                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the view model has been loaded. 
        /// </summary>
        public bool IsViewLoaded { get; private set; }

        /// <summary>
        /// Registers a <see cref="CancellationTokenSource"/> which will be cancelled when cleaning up the view model. 
        /// </summary>
        /// <param name="cancellationTokenSource"></param>
        public void RegisterCancellationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            if (_cancellationTokenSources == null)
                _cancellationTokenSources = new List<CancellationTokenSource>();

            _cancellationTokenSources.Add(cancellationTokenSource);
        }

        /// <summary>
        /// Creates a <see cref="CancellationTokenSource"/> and registers it if not disabled. 
        /// </summary>
        /// <param name="registerToken"></param>
        public CancellationTokenSource CreateCancellationTokenSource(bool registerToken = true)
        {
            var token = new CancellationTokenSource();
            if (registerToken)
                RegisterCancellationTokenSource(token);
            return token;
        }

        /// <summary>
        /// Runs a task and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. 
        /// </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public async Task<TResult> RunTaskAsync<TResult>(Func<CancellationToken, Task<TResult>> task)
        {
            TResult result = default(TResult);
            var tokenSource = CreateCancellationTokenSource();
            try
            {
                IsLoading = true;
                result = await task(tokenSource.Token);
                IsLoading = false;
            }
            catch (OperationCanceledException)
            {
                IsLoading = false;
            }
            catch (Exception exception)
            {
                IsLoading = false;
                HandleException(exception);
            }
            DeregisterCancellationTokenSource(tokenSource);
            return result;
        }

        /// <summary>
        /// Runs a task and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. 
        /// </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public Task RunTaskAsync(Func<CancellationToken, Task> task)
        {
            return RunTaskAsync(async token =>
            {
                await task(token);
                return (object)null;
            });
        }

        /// <summary>
        /// Runs a task and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. 
        /// </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public Task RunTaskAsync(Func<Task> task)
        {
            return RunTaskAsync(async token =>
            {
                await task();
                return (object)null;
            });
        }

        /// <summary>
        /// Runs a task and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. 
        /// </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public Task<TResult> RunTaskAsync<TResult>(Func<Task<TResult>> task)
        {
            return RunTaskAsync(async token => await task());
        }

        /// <summary>Runs a task and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public async Task<TResult> RunTaskAsync<TResult>(Task<TResult> task)
        {
            TResult result = default(TResult);
            try
            {
                IsLoading = true;
                result = await task;
                IsLoading = false;
            }
            catch (OperationCanceledException)
            {
                IsLoading = false;
            }
            catch (Exception exception)
            {
                IsLoading = false;
                HandleException(exception);
            }
            return result;
        }

        /// <summary>Asynchronously runs an action and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public Task RunTaskAsync(Task task)
        {
            return RunTaskAsync(async () =>
            {
                await task;
                return (object)null;
            });
        }

        /// <summary>Asynchronously runs an action and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="action">The action to run. </param>
        /// <returns>The awaitable task. </returns>
        public Task RunTaskAsync(Action action)
        {
            return RunTaskAsync(
                Task.Run(action)
            );
        }

        /// <summary>Asynchronously runs an action and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="action">The action to run. </param>
        /// <returns>The awaitable task. </returns>
        public async Task<T> RunTaskAsync<T>(Func<T> action)
        {
            return await RunTaskAsync(
                Task.Run(action)
            );
        }

        /// <summary>Handles an exception which occured in the <c>RunTaskAsync</c> method. </summary>
        /// <param name="exception">The exception to handle. </param>
        public virtual void HandleException(Exception exception)
        {
            throw new NotImplementedException("An exception occured in RunTaskAsync. Override ViewModelBase.HandleException to handle this exception. ", exception);
        }

        /// <summary>Disposes and deregisters a <see cref="CancellationTokenSource"/>. 
        /// Should be called when the task has finished cleaning up the view model. </summary>
        /// <param name="cancellationTokenSource"></param>
        public void DeregisterCancellationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
            catch{ /* ignored */ }

            _cancellationTokenSources.Remove(cancellationTokenSource);
        }

        /// <summary>
        /// Initializes the view model (should be called in the view's Loaded event). 
        /// </summary>
        public async Task CallOnLoaded()
        {
            if (!IsViewLoaded)
            {
                await OnLoadedAsync();
                IsViewLoaded = true;
            }
        }

        /// <summary>Cleans up the view model (should be called in the view's Unloaded event). </summary>
        public async Task CallOnUnloaded()
        {
            if (IsViewLoaded)
            {
                await OnUnloadedAsync();
                IsViewLoaded = false;
            }

            CancelAllRunningTasks();
        }

        private void CancelAllRunningTasks()
        {
            if (_cancellationTokenSources != null)
            {
                foreach (var cancellationTokenSource in _cancellationTokenSources.ToArray())
                    DeregisterCancellationTokenSource(cancellationTokenSource);
            }
        }

        /// <summary>Implementation of the initialization method. 
        /// If the view model is already initialized the method is not called again by the Initialize method. </summary>
        protected virtual Task OnLoadedAsync()
        {
            // Must be empty
            return _emptyTask;
        }

        /// <summary>Implementation of the clean up method. 
        /// If the view model is already cleaned up the method is not called again by the Cleanup method. </summary>
        protected virtual Task OnUnloadedAsync()
        {
            // Must be empty
            return _emptyTask;
        }

        #endregion

    }
}
