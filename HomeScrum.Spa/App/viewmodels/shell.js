define(['plugins/router', 'durandal/app'], function (router, app) {
   return {
      router: router,
      search: function () {
         //It's really easy to show a message box.
         //You can add custom options too. Also, it returns a promise for the user's response.
         app.showMessage('Search not yet implemented...');
      },
      activate: function () {
         router.map([
             { route: '', title: 'Welcome', moduleId: 'viewmodels/welcome', nav: true },
             { route: 'flickr', moduleId: 'viewmodels/flickr', nav: true },
             { route: 'workItemTypes', title: 'Work Item Types', moduleId: 'viewmodels/workItemTypes', nav: true },
             { route: 'workItemStatuses', title: 'Work Item Statuses', moduleId: 'viewmodels/workItemStatuses', nav: true },
             { route: 'logout', title: 'Logout', moduleId: 'viewmodels/logout', nav: true, hash: 'account/logout' }
         ]).buildNavigationModel();

         return router.activate();
      }
   };
});