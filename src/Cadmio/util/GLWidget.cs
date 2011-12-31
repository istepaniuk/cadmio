using System;
using System.Collections;
using Gdk;
using Gtk;
using Tao.OpenGl;
using gl = Tao.OpenGl.Gl;
using glu = Tao.OpenGl.Glu;
	
namespace GtkGL
{

	public class GLWidget : GLArea
	{
		private int height;
		private int width;
		public float MouseX, MouseY;
		private bool button1Pressed = false;
		private double beginX = 0;
		private double beginY = 0;
		private double mX = 0;
		private double mY = 0;
		private float cameraAngleX = 0f;
		private float cameraAngleY = 0f;
		private float cameraDistance = 1;
		private Gdk.Cursor cursorCross = new Gdk.Cursor (Gdk.CursorType.Crosshair);
		private Gdk.Cursor cursorDrag = new Gdk.Cursor (Gdk.CursorType.Dot);
		
		private enum MouseOpMode
		{
			None,
			Drag,
			Zoom
		}

		private MouseOpMode mouseMode;

		static System.Int32[] attrlist = { (int)GtkGL._GDK_GL_CONFIGS.Rgba, (int)GtkGL._GDK_GL_CONFIGS.RedSize, 1, (int)GtkGL._GDK_GL_CONFIGS.GreenSize, 1, (int)GtkGL._GDK_GL_CONFIGS.BlueSize, 1, (int)GtkGL._GDK_GL_CONFIGS.DepthSize, 1, (int)GtkGL._GDK_GL_CONFIGS.Doublebuffer,
		(int)GtkGL._GDK_GL_CONFIGS.None };

		protected ArrayList GLObjectList;

		public void AddGLObject (IGLObject ob)
		{
			if (TreeUpdated != null)
				TreeUpdated (this, null);
			ob.Updated += OnExposed;
			// init here?
			ob.Init ();
			GLObjectList.Add (ob);
		}

		private void Init ()
		{
			// The GL widget has a minimum size of 300x300 pixels 
			this.SetSizeRequest (300, 300);
			
			// Initialize the GLObjectList
			GLObjectList = new ArrayList ();
			
			// Connect some other signals		
			this.ExposeEvent += OnExposed;
			this.Realized += OnRealized;
			this.SizeAllocated += OnSizeAllocated;
			this.ConfigureEvent += OnConfigure;
			this.MotionNotifyEvent += OnMotionNotifyEvent;
			this.EnterNotifyEvent += OnEnterNotifyEvent;
			this.LeaveNotifyEvent += OnLeaveNotifyEvent;
			this.ButtonPressEvent += OnButtonPressEvent;
			this.ButtonReleaseEvent += OnButtonReleaseEvent;
			this.ScrollEvent += OnScrollEvent;
			this.Events = Gdk.EventMask.PointerMotionMask | Gdk.EventMask.PointerMotionHintMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ScrollMask;
		}

		public GLWidget () : base(attrlist)
		{
			this.Init ();
		}

		public GLWidget (System.Int32[] attrList) : base(attrList)
		{
			this.Init ();
		}

		// This handler gets fired when the glArea widget is re-sized
		void OnSizeAllocated (object o, Gtk.SizeAllocatedArgs e)
		{
			height = e.Allocation.Height;
			width = e.Allocation.Width;
			float parentRel = (float)width / height;
			Console.WriteLine (parentRel);
			
			// Avoid devide-by-zero error
			if (height == 0) {
				height = 1;
			}
			gl.glViewport (0, 0, width, height);
			gl.glMatrixMode (gl.GL_PROJECTION);
			gl.glLoadIdentity ();
			glu.gluPerspective (45.0f, (float)width / (float)height, 1f, 100.0f);
			gl.glTranslatef (0.0f, 0.0f, -10.0f);
			gl.glMatrixMode (gl.GL_MODELVIEW);
			gl.glLoadIdentity ();
			
		}

		protected void OnExposed (object o, EventArgs e)
		{
			Redraw ();
		}

		private void DrawGrid (float size, float step)
		{
			gl.glDisable (gl.GL_LIGHTING);
			gl.glBegin (gl.GL_LINES);
			gl.glLineWidth (1.5f);
			gl.glColor4f (0.3f, 0.3f, 0.3f, 0.1f);
			for (float i = -size; i <= size; i += step) {
				gl.glVertex3f (i, -size, 0);
				gl.glVertex3f (i, size, 0);
				gl.glVertex3f (-size, i, 0);
				gl.glVertex3f (size, i, 0);
			}
			gl.glEnd ();
			gl.glEnable (gl.GL_LIGHTING);
		}


		protected void Redraw ()
		{
			if (this.MakeCurrent () == 0)
				return;
			
			gl.glClearColor (0.9f, 0.9f, 1f, 1f);
			gl.glClear (gl.GL_COLOR_BUFFER_BIT | gl.GL_DEPTH_BUFFER_BIT);
			
			gl.glMatrixMode (gl.GL_MODELVIEW);
			gl.glPushMatrix ();
			// Prepare Dynamic Transform
			gl.glLoadIdentity ();
			
			gl.glTranslatef (0, 0, -cameraDistance);
			gl.glRotatef (cameraAngleY, 1, 0, 0);
			// pitch
			gl.glRotatef (cameraAngleX, 0, 0, 1);
			// heading
			
			float[] position = { -10f, -10f, -10f, 1f };
			Gl.glLightfv (Gl.GL_LIGHT0, Gl.GL_POSITION, position);
			
			// Draw the GLObjects associated with this GLArea
			foreach (GtkGL.IGLObject glObj in GLObjectList) {
				if (!(glObj is Cadmio.Plane))
					glObj.Draw ();
			}
			
			DrawGrid (10, 0.5f);
			
			foreach (GtkGL.IGLObject glObj in GLObjectList) {
				if (glObj is Cadmio.Plane)
					glObj.Draw ();
			}
			
			double[] groundPlane = new double[4] { 0, 0, 1, 0 };
			
			double[] glPoint = new double[3] { 0, 0, 0 };
			if (mouseMode != GLWidget.MouseOpMode.Drag)
				glPoint = calculateIntersect (mX, mY, groundPlane);
			MouseX = (float)glPoint[0];
			MouseY = (float)glPoint[1];
			if (MouseMove != null)
				MouseMove (this, null);
			
			// Pojected cursor
			gl.glColor3f (1f, 1f, 1f);
			gl.glBegin (gl.GL_LINES);
			gl.glVertex3f ((float)glPoint[0]-0.5f, (float)glPoint[1], 0);
			gl.glVertex3f ((float)glPoint[0]+0.5f, (float)glPoint[1], 0);
			gl.glVertex3f ((float)glPoint[0], (float)glPoint[1] -0.5f, 0);
			gl.glVertex3f ((float)glPoint[0], (float)glPoint[1] +0.5f , 0);
			gl.glEnd ();

			gl.glPopMatrix ();
			
			// Bring back buffer to front, put front buffer in back
			SwapBuffers ();
		}


		public static void calculateScreenRay (double x, double y, ref Vector3 vRayPosOut, ref Vector3 vRayDirOut)
		{
			int[] viewport = new int[4];
			double[] modelview = new double[16];
			double[] projection = new double[16];
			gl.glGetDoublev (gl.GL_MODELVIEW_MATRIX, modelview);
			gl.glGetDoublev (gl.GL_PROJECTION_MATRIX, projection);
			gl.glGetIntegerv (gl.GL_VIEWPORT, viewport);
			
			double[] fOut = new double[4];
			
			
			glu.gluUnProject ((double)x, (double)(viewport[3] - y), (double)0.1, modelview, projection, viewport, out fOut[0], out fOut[1], out fOut[2]);
			
			
			vRayPosOut.X = (float)fOut[0];
			vRayPosOut.Y = (float)fOut[1];
			vRayPosOut.Z = (float)fOut[2];
			
			glu.gluUnProject ((double)x, (double)(viewport[3] - y), (double)0.9, modelview, projection, viewport, out fOut[0], out fOut[1], out fOut[2]);
			
			vRayDirOut.X = (float)fOut[0];
			vRayDirOut.Y = (float)fOut[1];
			vRayDirOut.Z = (float)fOut[2];
			
			
			vRayDirOut -= vRayPosOut;
			vRayDirOut.Normalize ();
		}


		public static double[] calculateIntersect (double x, double y, double[] plane)
		{
			Vector3 vRayPos = new Vector3 ();
			Vector3 vRayDir = new Vector3 ();
			
			calculateScreenRay (x, y, ref vRayPos, ref vRayDir);
			
			// Finally set the mouse world position to the intersection location
			return rayPlaneIntersect (vRayPos, vRayDir, plane);
		}


		public static double[] rayPlaneIntersect (Vector3 vRayPos, Vector3 vRayDir, double[] plane)
		{
			Vector3 planeNormal = new Vector3 (plane[0], plane[1], plane[2]);
			
			double denom = vRayDir.DotProduct (planeNormal);
			// Calculate the denominator first
			//if( Math.Abs(denom) < 0.0001f ) return null;                      // Make sure we won't create a divide by zero
			
			double numer = vRayPos.DotProduct (planeNormal) + plane[3];
			// Calculate the numerator next
			System.Console.WriteLine (denom.ToString () + "  " + numer.ToString ());
			
			double time = -numer / denom;
			// Get the time of intersection
			Vector3 result = new Vector3 ((time * vRayDir) + vRayPos);
			// Return the point of intersection
			return result.Position ();
		}

		public void Initworkspace ()
		{
			EnableLighting (null, null);
			Redraw ();
		}

		public void ZoomIn ()
		{
			cameraDistance += 1;
			Redraw ();
			
		}
		public void ZoomOut ()
		{
			cameraDistance -= 1;
			Redraw ();
		}

		public void TestThing ()
		{
			Cadmio.Plane XYPlane = new Cadmio.Plane (0, new Vector3 (1, 1, 0));
			Cube cube = new Cube ();
			this.AddGLObject (cube);
			Cadmio.Origin Origin = new Cadmio.Origin (1);
			Cadmio.Axis XAxis = new Cadmio.Axis (2, new Vector3 (1, 0, 0), Origin.Position);
			
			Cadmio.Point PointA = new Cadmio.Point (3, new Vector3 (2, 2, 2));
			
			AddGLObject (Origin);
			AddGLObject (XAxis);
			AddGLObject (XYPlane);
			AddGLObject (PointA);
			
			
			Console.WriteLine (XYPlane.Name);
			Redraw ();
		}

		public static void EnableLighting (object o, EventArgs e)
		{
			float[] position = { -10f, -10f, -10f, 1f };
			
			Gl.glEnable (Gl.GL_LIGHT0);
			Gl.glLightfv (Gl.GL_LIGHT0, Gl.GL_POSITION, position);
			
			float[] color = { 1f, 1f, 1f, 0f };
			
			Gl.glEnable (Gl.GL_LIGHT1);
			Gl.glLightfv (Gl.GL_LIGHT1, Gl.GL_AMBIENT, color);
			Gl.glEnable (Gl.GL_LIGHTING);
			
		}

		public event EventHandler MouseMove;

		// Connect a method to this handler to extend the state setup
		public event EventHandler GLSetup;

		public event EventHandler TreeUpdated;

		// One-time configuration of opengl states happens here
		protected virtual void InitGL ()
		{
			// Run the associated Setup methods first
			if (GLSetup != null)
				GLSetup (this, null);
			
			gl.glShadeModel (gl.GL_SMOOTH);
			// Enables Smooth Shading
			gl.glClearColor (0.0f, 0.0f, 0.0f, 0.0f);
			gl.glClearDepth (1.0f);
			
			Gl.glEnable (Gl.GL_AUTO_NORMAL);
			Gl.glEnable (Gl.GL_NORMALIZE);
			gl.glEnable (gl.GL_DEPTH_TEST);
			// Enables Depth Testing
			gl.glEnable (gl.GL_BLEND);
			
			gl.glEnable (gl.GL_COLOR_MATERIAL);
			gl.glEnable (gl.GL_LINE_SMOOTH);
			
			gl.glBlendFunc (gl.GL_SRC_ALPHA, gl.GL_ONE_MINUS_SRC_ALPHA);
			gl.glDepthFunc (gl.GL_LEQUAL);
			// The Type Of Depth Test To Do
			// Really Nice Perspective Calculations
			
			gl.glHint (gl.GL_PERSPECTIVE_CORRECTION_HINT, gl.GL_NICEST);
			gl.glHint (gl.GL_LINE_SMOOTH_HINT, gl.GL_NICEST);
			gl.glLineWidth (1.5f);
			
			/*
			float[] materialSpecular = { 1.0f, 1.0f, 1.0f, 0.15f };
			float[] materialShininess = { 100.0f };
			int[] materialEmission = { 0, 0, 0, 255 };
			
			gl.glMaterialiv (gl.GL_FRONT_AND_BACK, gl.GL_EMISSION, materialEmission);
			Gl.glMaterialfv (Gl.GL_FRONT, Gl.GL_SPECULAR, materialSpecular);
			Gl.glMaterialfv (Gl.GL_FRONT, Gl.GL_SHININESS, materialShininess);
			*/
			Gl.glFrontFace (Gl.GL_CW);
			gl.glShadeModel (gl.GL_SMOOTH);
			// Really Nice Perspective Calculations
			gl.glHint (gl.GL_PERSPECTIVE_CORRECTION_HINT, gl.GL_NICEST);

			return;
		}

		// Fired off when the widget is realized
		void OnRealized (object o, EventArgs e)
		{
			if (this.MakeCurrent () == 0)
				return;
			
			// Run the state setup routine
			InitGL ();
			
			// Iterate over associated IGLObject objects, calling Init() on each
			System.Collections.IEnumerator enumerator = GLObjectList.GetEnumerator ();
			
			while (enumerator.MoveNext ()) {
				((GtkGL.IGLObject)enumerator.Current).Init ();
			}
		}

		// This handler gets fired when the glArea widget is re-sized
		void OnConfigure (object o, EventArgs e)
		{
			if (this.MakeCurrent () == 0)
				return;
			
			gl.glViewport (0, 0, this.Allocation.Width, this.Allocation.Height);
		}

		// Bound in glwidget.glade		
		private void OnQuit (object o, System.EventArgs e)
		{
			Application.Quit ();
		}

		// Bound in glwidget.glade
		private void OnWindowDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		private void OnMotionNotifyEvent (object sender, MotionNotifyEventArgs a)
		{
			
			Gdk.EventMotion ev = a.Event;
			
			//if ((ev.State & Gdk.ModifierType.Button1Mask) != 0) {
			if (button1Pressed) {
				mouseMode = GLWidget.MouseOpMode.Drag;
				GdkWindow.Cursor = cursorDrag;
			} else {
				mouseMode = GLWidget.MouseOpMode.None;
				GdkWindow.Cursor = cursorCross;
			}
			
			/*
			testX = (float) (ev.X - width / 2f) / width *2f ;
			testY = (float) -(ev.Y - height/ 2f) / height*2f ;
			Console.WriteLine("state: "+ev.State.ToString());
			*/

			switch (mouseMode) {
				case MouseOpMode.Drag:
					
					int ix, iy;
					double x, y;
					
					Gdk.ModifierType m;
					
					// Find the current mouse X and Y positions
					if (ev.IsHint) {
						ev.Window.GetPointer (out ix, out iy, out m);
						x = (double)ix;
						y = (double)iy;
					} else {
						x = ev.X;
						y = ev.Y;
					}

					cameraAngleX += (float)(x - beginX);
					cameraAngleY += (float)(y - beginY);
					// Reset the "old" X and Y positions
					beginX = x;
					beginY = y;
					break;
			}
			mX = ev.X;
			mY = ev.Y;
			Redraw ();
			
		}
		private void OnButtonPressEvent (object sender, ButtonPressEventArgs a)
		{
			Gdk.EventButton ev = a.Event;
			
			if (ev.Button == 1) {
				button1Pressed = true;
				/* potential beginning of drag, reset mouse position */
				beginX = ev.X;
				beginY = ev.Y;
				return;
			}
			Console.WriteLine (ev.Button.ToString ());
			
		}
		private void OnScrollEvent (object sender, ScrollEventArgs a)
		{
			Gdk.EventScroll ev = a.Event;
			if (ev.Direction == ScrollDirection.Up)
				ZoomIn ();
			else
				ZoomOut ();
			Redraw ();
		}

		private void OnEnterNotifyEvent (object sender, EnterNotifyEventArgs a)
		{
			//GdkWindow.Cursor = cursorCross;
		}
		private void OnLeaveNotifyEvent (object sender, LeaveNotifyEventArgs a)
		{
			//GdkWindow.Cursor = cursorDefault;
		}
		private void OnButtonReleaseEvent (object sender, ButtonReleaseEventArgs a)
		{
			if (a.Event.Button == 1) {
				button1Pressed = false;
			}
		}
	}
}