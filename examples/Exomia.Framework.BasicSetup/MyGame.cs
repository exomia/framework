#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Game;
using Exomia.IoC;
using Exomia.IoC.Attributes;
using Exomia.Logging;

namespace Exomia.Framework.BasicSetup
{
    /// <summary>
    ///     My game class. This class cannot be inherited.
    /// </summary>
    internal sealed class MyGame : Game
    {
        /// <summary> Initializes a new instance of the <see cref="MyGame" /> class. </summary>
        /// <param name="serviceProvider"> The service provider. </param>
        /// <param name="logger">          The logger. </param>
        public MyGame(IServiceProvider                                                   serviceProvider,
                      [IoCOptional(typeof(SimpleConsoleLogger<MyGame>))] ILogger<MyGame> logger)
            : base(serviceProvider)
        {
            //IsFixedTimeStep   = true;
            //TargetElapsedTime = 1000.0 / 144;

            logger.Log(LogLevel.Trace,       null, "ctor for 'MyGame' called...");
            logger.Log(LogLevel.Debug,       null, "ctor for 'MyGame' called...");
            logger.Log(LogLevel.Information, null, "ctor for 'MyGame' called...");
            logger.Log(LogLevel.Warning,     null, "ctor for 'MyGame' called...");
            logger.Log(LogLevel.Error,       null, "ctor for 'MyGame' called...");
            logger.Log(LogLevel.Critical,    null, "ctor for 'MyGame' called...");
        }
    }
}