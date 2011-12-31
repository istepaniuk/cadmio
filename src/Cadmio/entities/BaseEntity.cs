using GtkGL;

namespace Cadmio
{
	
	public abstract class BaseEntity : GtkGL.GLObjectBase
	{
		public int ID;
		public bool Visible = true;
		public bool Locked = false;
		public string Name;
				
		public BaseEntity (int ID)
		{
			Name = "Entity " + ID.ToString();
		}

		public void Delete(){
		  
		}
	}
}