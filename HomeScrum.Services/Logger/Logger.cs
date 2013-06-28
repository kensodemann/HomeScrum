using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Services.Logger
{
   public class Logger : ILogger
   {
      [Inject]
      public Logger(Ninject.Extensions.Logging.ILogger logger)
      {
         _logger = logger;
      }

      private readonly Ninject.Extensions.Logging.ILogger _logger;


      public void Debug( string message )
      {
         _logger.Debug( message );
      }

      public void Debug( string format, params object[] args )
      {
         _logger.Debug( format, args );
      }

      public void Debug( Exception exception, string format, params object[] args )
      {
         _logger.Debug( exception, format, args );
      }

      public void Error( string message )
      {
         _logger.Error( message );
      }

      public void Error( string format, params object[] args )
      {
         _logger.Error( format, args );
      }

      public void Error( Exception exception, string format, params object[] args )
      {
         _logger.Error( exception, format, args );
      }

      public void Fatal( string message )
      {
         _logger.Fatal( message );
      }

      public void Fatal( string format, params object[] args )
      {
         _logger.Fatal( format, args );
      }

      public void Fatal( Exception exception, string format, params object[] args )
      {
         _logger.Fatal( exception, format, args );
      }

      public void Info( string message )
      {
         _logger.Info( message );
      }

      public void Info( string format, params object[] args )
      {
         _logger.Info( format, args );
      }

      public void Info( Exception exception, string format, params object[] args )
      {
         _logger.Info( exception, format, args );
      }

      public void Trace( string message )
      {
         _logger.Trace( message );
      }

      public void Trace( string format, params object[] args )
      {
         _logger.Trace( format, args );
      }

      public void Trace( Exception exception, string format, params object[] args )
      {
         _logger.Trace( exception, format, args );
      }

      public void Warn( string message )
      {
         _logger.Warn( message );
      }

      public void Warn( string format, params object[] args )
      {
         _logger.Warn( format, args );
      }

      public void Warn( Exception exception, string format, params object[] args )
      {
         _logger.Warn( exception, format, args );
      }
   }
}
