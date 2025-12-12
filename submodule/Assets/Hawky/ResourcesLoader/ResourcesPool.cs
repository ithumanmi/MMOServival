using UnityEngine;

namespace Hawky.ResourcesLoader
{
    public class Model
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public Model(string id)
        {
            this._id = id;
        }
    }

    public class ResourcesPool : MonoBehaviour
    {
        private Model _model;

        public string GetId()
        {
            if (_model == null)
            {
                return string.Empty;
            }

            return _model.Id;
        }

        public void Init(Model model)
        {
            _model = model;
            OnInit(model);
        }

        protected virtual void OnInit(Model model)
        {
            _model = model;
        }

        [HideInInspector]
        public bool free = true;

        public void Created()
        {
            OnCreated();
        }

        public void Free()
        {
            OnFree();
            free = true;
            gameObject.SetActive(false);
        }

        public void Use()
        {
            free = false;
            gameObject.SetActive(true);
            OnUse();
        }

        protected virtual void OnCreated()
        {

        }

        protected virtual void OnFree()
        {

        }

        protected virtual void OnUse()
        {

        }
    }
}
