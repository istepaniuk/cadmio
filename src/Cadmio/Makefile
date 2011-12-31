
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG_X86)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG" "-main:Cadmio.MainClass"
ASSEMBLY = bin/Debug/Cadmio.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = winexe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

CADMIO_EXE_MDB_SOURCE=bin/Debug/Cadmio.exe.mdb
CADMIO_EXE_MDB=$(BUILD_DIR)/Cadmio.exe.mdb

endif

ifeq ($(CONFIG),RELEASE_X86)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- "-main:Cadmio.MainClass"
ASSEMBLY = bin/Release/Cadmio.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = winexe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

CADMIO_EXE_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(CADMIO_EXE_MDB)  

BINARIES = \
	$(CADMIO)  


RESGEN=resgen2

CADMIO = $(BUILD_DIR)/cadmio

FILES =  \
	AssemblyInfo.cs \
	entities/Axis.cs \
	entities/BaseEntity.cs \
	entities/Origin.cs \
	entities/Plane.cs \
	entities/Point.cs \
	gtk-gui/generated.cs \
	gtk-gui/MainWindow.cs \
	Main.cs \
	MainWindow.cs \
	shapes/ColoredSquare.cs \
	shapes/ColoredTriangle.cs \
	shapes/Cube.cs \
	shapes/Pyramid.cs \
	shapes/Square.cs \
	shapes/Teapot.cs \
	shapes/Triangle.cs \
	util/EulerRotation.cs \
	util/GLObjectBase.cs \
	util/GLWidget.cs \
	util/IGLObject.cs \
	util/Quaternion.cs \
	util/Rotation.cs \
	util/TransformationMatrix.cs \
	util/Vector.cs \
	util/Vector3.cs 

DATA_FILES = 

RESOURCES =  \
	gtk-gui/gui.stetic \
	icons/16x16/grid.png,Cadmio.icons.16x16.grid.png \
	icons/16x16/plane.png,Cadmio.icons.16x16.plane.png \
	icons/16x16/system.png,Cadmio.icons.16x16.system.png \
	icons/24x24/grid.png,Cadmio.icons.24x24.grid.png \
	icons/24x24/plane.png,Cadmio.icons.24x24.plane.png \
	icons/24x24/system.png,Cadmio.icons.24x24.system.png 

EXTRAS = \
	entities \
	cadmio.in 

REFERENCES =  \
	Mono.Posix \
	-pkg:glade-sharp-2.0 \
	-pkg:glib-sharp-2.0 \
	-pkg:gtkglarea-sharp \
	-pkg:gtk-sharp-2.0 \
	-pkg:monodevelop \
	-pkg:tao-opengl \
	System \
	System.Xml

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-wrapper,CADMIO,cadmio,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY_MDB): $(ASSEMBLY)
$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(CADMIO_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(bindir)'
	$(call cp,$(CADMIO),$(DESTDIR)$(bindir))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(CADMIO_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(CADMIO),$(DESTDIR)$(bindir))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
