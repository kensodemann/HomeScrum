The code in this folder was obtained from here: https://code.google.com/p/unhaddins/source/browse/#hg%2FuNhAddIns%2FuNhAddIns.NinjectAdapters%2FBytecodeProvider

It was used in conjunction with this article: http://nhforge.org/blogs/nhibernate/archive/2008/12/12/entities-behavior-injection.aspx

This was done in order to facilitate injecting stuff into the domain entities.  However, that is really not required at this time.

The only thing I want to inject is the ISessionFactory, and then only for validation.
The objects that the validations are actually run on are not created by NHibernate, but by AutoMapper, which already uses Ninject.