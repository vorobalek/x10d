using System;
using System.Collections.Generic;
using System.Reflection;

namespace X10D.Infrastructure
{
    /// <summary>
    /// Метадата модуля
    /// </summary>
    public abstract class BaseMetadata : ExtCore.Infrastructure.ExtensionBase
    {
        /// <summary>
        /// Необходимая версия модуля. Устанавливается только в абстрактном классе <see cref="BaseMetadata"/>.
        /// </summary>
        public Version RequiredVersion => new Version(0, 0, 0, 1);

        /// <summary>
        /// Имя текущей сборки модуля.
        /// </summary>
        public string Name => Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// Описание текущей сборки модуля.
        /// </summary>
        public virtual string Description => $"Модуль системы X10D. {Name}";

        /// <summary>
        /// Автор(-ы) текущей сборки модуля.
        /// </summary>
        public virtual string Authors => "";

        /// <summary>
        /// Имя основной сборки модуля.
        /// </summary>
        public virtual string Owner => "";

        /// <summary>
        /// Зависимые сборки <see cref="BaseMetadata"/>.
        /// </summary>
        [Obsolete("Не реализовано.", true)]
        public IEnumerable<BaseMetadata> ReferencesModules => throw new NotImplementedException();

        /// <summary>
        /// Зависимые сборки <see cref="Assembly"/>.
        /// </summary>
        [Obsolete("Не реализовано.", true)]
        public IEnumerable<Assembly> ReferencesAssemblies => throw new NotImplementedException();

        /// <summary>
        /// Сборки подмодулей.
        /// </summary>
        [Obsolete("Не реализовано.", true)]
        public virtual IEnumerable<BaseMetadata> SubModules => throw new NotImplementedException();

        /// <summary>
        /// Получить версию модуля.
        /// </summary>
        /// <returns>Актуальная версия модуля.</returns>
        public Version GetVersion()
        {
            return Version;
        }

        /// <summary>
        /// Гарантированная версия совместимости модуля.
        /// </summary>
        new protected virtual Version Version => new Version(0, 0, 0, 0);
    }
}
