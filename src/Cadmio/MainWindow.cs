using System;
using Gtk;
using GtkGL;
using Tao.OpenGl;
using gl = Tao.OpenGl.Gl;
using glu = Tao.OpenGl.Glu;

public partial class MainWindow : Gtk.Window
{
	public GtkGL.GLWidget glw;
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		this.Title = "Cadmio";
		glw = new GLWidget();
	 	glw.MotionNotifyEvent += OnGlMotionNotifyEvent;
		glw.MouseMove += OnGLMouseMove;
		glw.TreeUpdated += OnGLTreeUpdated;
			
		vbGl.PackStart (glw,true,true,0);
		ShowAll();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
	protected virtual void OnNewActionActivated (object sender, System.EventArgs e)
	{
		glw.Initworkspace();
	}
	
	protected virtual void OnRefreshActionActivated (object sender, System.EventArgs e)
	{
		glw.TestThing();
	}

	protected virtual void OnGlMotionNotifyEvent (object o, Gtk.MotionNotifyEventArgs args)
	{
		
	}
	
	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		Application.Quit();
	}
	
	protected virtual void OnGLMouseMove (object sender, System.EventArgs e)
	{
		statusbar.Pop(0);
		statusbar.Push(0,glw.MouseX.ToString("0.000") + "," + glw.MouseY.ToString("0.000") );
		
	}
	
	protected virtual void OnGLTreeUpdated (object sender, System.EventArgs e)
	{
		Gtk.TreeViewColumn nameColumn = new Gtk.TreeViewColumn ("Name",new Gtk.CellRendererText(),"text", 1);
		Gtk.TreeViewColumn iconColum = new Gtk.TreeViewColumn ("Icon", new Gtk.CellRendererPixbuf (), "pixbuf", 0);
		
		treeview.AppendColumn(iconColum);
		treeview.AppendColumn(nameColumn);
	
		Gtk.TreeStore objectsTreeStore = new Gtk.TreeStore (typeof (Gdk.Pixbuf), typeof (string));
		
		treeview.Model = objectsTreeStore;
	
		Gdk.Pixbuf pixSystem = Gdk.Pixbuf.LoadFromResource("Cadmio.icons.16x16.system.png");
		Gdk.Pixbuf pixPlane = Gdk.Pixbuf.LoadFromResource("Cadmio.icons.16x16.plane.png");
		Gdk.Pixbuf pixGrid = Gdk.Pixbuf.LoadFromResource("Cadmio.icons.16x16.grid.png");
		
		Gtk.TreeIter iter = objectsTreeStore.AppendValues (pixSystem, "System");
		objectsTreeStore.AppendValues (iter, pixPlane, "Origin");
		objectsTreeStore.AppendValues (iter, pixGrid, "Grid");
		
		treeview.ExpandAll();
	}
	
	protected virtual void OnExecuteActionActivated (object sender, System.EventArgs e)
	{

	}
}