
namespace MiniORM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    // TODO: Create your ChangeTracker class here.
    class ChangeTracker<T>
			where T: class,new()
    {
		private readonly List<T> allEntities;

		private readonly List<T> added;

		private readonly List<T> removed;

		
		public ChangeTracker(IEnumerable<T> entities)
        {
			this.added = new List<T>();
			this.removed = new List<T>();

			this.allEntities = CloneEntities(entities);
        }

        public IReadOnlyCollection<T> AllEntities => this.allEntities.AsReadOnly();

        public IReadOnlyCollection<T> Added => this.added.AsReadOnly();

        public IReadOnlyCollection<T> Removed => this.removed.AsReadOnly();


        public void Add(T item) => this.added.Add(item);

        public void Remove(T item) => this.removed.Add(item);


		private static List<T> CloneEntities(IEnumerable<T> entities)
        {
			List<T> clonedEntities = new List<T>();

			PropertyInfo[] propertiesToClone = typeof(T)
				.GetProperties()
				.Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType)).ToArray();

            foreach (T entity in entities)
            {
				T clonedEntity = Activator.CreateInstance<T>();

                foreach (PropertyInfo property in propertiesToClone)
                {
                    object value = property.GetValue(entity);
                    property.SetValue(obj: clonedEntities, value);
                }
                clonedEntities.Add(clonedEntity);
            }
            return clonedEntities;
        }
    }
}