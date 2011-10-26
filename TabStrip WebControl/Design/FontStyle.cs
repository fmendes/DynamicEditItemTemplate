using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.ComponentModel;

namespace SCS.Web.UI.WebControls.Design
{
    public class TextStyle : Component, IStateManager
    {
        private bool _isTrackingViewState;

        private StateBag _viewState;
        private Color _color;
        private FontInfo _fontInfo;

        protected StateBag ViewState
        {
            get
            {
                if (_viewState == null)
                {
                    _viewState = new StateBag(false);
                    if (_isTrackingViewState) ((IStateManager)_viewState).TrackViewState();
                }
                return _viewState;
            }
        }
        internal void SetDirty()
        {
            if (_viewState != null)
            {
                ICollection Keys = _viewState.Keys;
                foreach (string key in Keys)
                {
                    _viewState.SetItemDirty(key, true);
                }
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public Color ForeColor 
        { 
            get { return _color; }
            set { _color = value; }
        }

        [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
        public FontInfo Font
        {
            get { return _fontInfo; }
            set { _fontInfo = value; }
        }

        #region IStatemanager implementation

        bool IStateManager.IsTrackingViewState
        {
            get
            {
                return _isTrackingViewState;
            }
        }
        void IStateManager.LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                ((IStateManager)ViewState).LoadViewState(savedState);

                //string text = (string)ViewState["text"];

                //if (text != null)
                //    this.Text = text;

                //string tag = (string)ViewState["tag"];

                //if (tag != null)
                //    this.Tag = tag;

                //string url = (string)ViewState["url"];

                //if (url != null)
                //    this.Url = url;

                //string id = (string)ViewState["id"];

                //if (id != null)
                //    this.Id = id;

                //object postBack = ViewState["postBack"];

                //if (postBack != null)
                //    this.PostBack = Convert.ToBoolean(postBack);

                //object selected = ViewState["selected"];

                //if (selected != null)
                //    this.Selected = Convert.ToBoolean(selected);
            }
        }
        object IStateManager.SaveViewState()
        {
            //string currentText = this.Text;
            //string initialText = (string)ViewState["text"];

            //if (currentText.Equals(initialText) == false)
            //{
            //    ViewState["text"] = currentText;
            //}

            //string currentTag = this.Tag;
            //string initialTag = (string)ViewState["tag"];

            //if (currentTag.Equals(initialTag) == false)
            //{
            //    ViewState["tag"] = currentTag;
            //}

            //string currentUrl = this.Url;
            //string initialUrl = (string)ViewState["url"];

            //if (currentUrl.Equals(initialUrl) == false)
            //{
            //    ViewState["url"] = currentUrl;
            //}

            //string currentId = this.Id;
            //string initialId = (string)ViewState["id"];

            //if (currentId.Equals(initialId) == false)
            //{
            //    ViewState["id"] = currentId;
            //}

            //bool currentPostBack = this.PostBack;
            //bool initialPostBackTyped = (ViewState["postBack"] == null) ? false : (bool)ViewState["postBack"];

            //if (currentPostBack != initialPostBackTyped)
            //{
            //    ViewState["postBack"] = currentPostBack;
            //}

            //bool currentSelected = this.Selected;
            //bool initialSelectedTyped = (ViewState["selected"] == null) ? false : (bool)ViewState["selected"];

            //if (currentSelected != initialSelectedTyped)
            //{
            //    ViewState["selected"] = currentSelected;
            //}

            if (_viewState != null)
            {
                return ((IStateManager)_viewState).SaveViewState();
            }
            return null;
        }
        void IStateManager.TrackViewState()
        {
            //if (_text.Length > 0)
            //    ViewState["text"] = _text;

            //if (_tag.Length > 0)
            //    ViewState["tag"] = _tag;

            //if (_url.Length > 0)
            //    ViewState["url"] = _url;

            //if (_id.Length > 0)
            //    ViewState["id"] = _id;

            //if (!_postBack)
            //    ViewState["postBack"] = !_postBack;

            //_isTrackingViewState = true;

            if (_viewState != null)
            {
                ((IStateManager)_viewState).TrackViewState();
            }
        }

        #endregion
    }
}
