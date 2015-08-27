using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication1.Model;

namespace WebApplication1
{
    public partial class Default : System.Web.UI.Page
    {
        private string _imageQuery;

        public string ImageQuery { get { return _imageQuery ?? (_imageQuery = Request.QueryString.ToString()); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            SuccessPlaceHolder.Visible = false;

            if (!String.IsNullOrWhiteSpace(ImageQuery))
            {
                BigImage.ImageUrl = Path.Combine("~/Content/images/", ImageQuery);  
                BigImage.Visible = true;
            }
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                Stream stream;
                string fileName = ImageFileUpload.PostedFile.FileName;

                stream = ImageFileUpload.PostedFile.InputStream;

                Gallery.SaveImage(stream, fileName);

                SuccessPlaceHolder.Visible = true;
            }
        }

        public IEnumerable<string> ThumbnailRepeater_GetData()
        {
            return Gallery.GetImageNames();
        }

    }
}