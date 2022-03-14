using Extension.Ext;
using Extension.Script;
using Extension.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Components
{
    [Serializable]
    public class ExtComponent<TExt> : Component where TExt : class, IExtension
    {
        public ExtComponent(TExt owner, int id, string name) : base(id)
        {
            _owner = owner;
            Name = name;

            _unstartedComponents = new List<Component>();
            _unstartedComponents.Add(this);
        }

        public TExt Owner => _owner;
        public event Action OnAwake;

        public override void Awake()
        {
            base.Awake();

            OnAwake?.Invoke();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (_unstartedComponents.Count > 0)
            {
                Component.ForeachComponents(_unstartedComponents, c => c.EnsureStarted());
                _unstartedComponents.Clear();
            }
        }

        protected override void AddComponent(Component component)
        {
            base.AddComponent(component);

            component.EnsureAwaked();
            _unstartedComponents.Add(component);
        }

        /// <summary>
        /// return myself and ensure awaked
        /// </summary>
        /// <returns></returns>
        public ExtComponent<TExt> GetAwaked()
        {
            EnsureAwaked();

            return this;
        }

        public void CreateScriptComponent<T>(int id, string description, params object[] parameters) where T : Scriptable<TExt>
        {
            var script = ScriptManager.GetScript(typeof(T).Name);
            var scriptComponent = ScriptManager.CreateScriptable<T>(script, parameters);
            scriptComponent.ID = id;
            scriptComponent.Name = description;

            scriptComponent.AttachToComponent(this);
        }
        public void CreateScriptComponent<T>(string description, params object[] parameters) where T : Scriptable<TExt>
        {
            CreateScriptComponent<T>(NO_ID, description, parameters);
        }

        ExtensionReference<TExt> _owner;
        List<Component> _unstartedComponents;
    }
}
