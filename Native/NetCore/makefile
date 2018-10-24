#
# if sys/cdefs.h is missing, you may need to install libc6-dev-i386 package (on ubuntu) or such.
#
.PHONY:	all clean libijg8 libijg12 libijg16
.SUFFIXES:
.SUFFIXES: .c .o .so


ALL_ARCHS	= linux-x64 linux-x86
ALL_LIBS	= libijg8 libijg12 libijg16
CFLAGS		+= -fPIC -O -shared -D NODEBUG -D '__declspec(T)='


ifeq ($(ARCH),)

all-archs:
	for arch in $(ALL_ARCHS); do	\
		$(MAKE) ARCH=$${arch};		\
	done

endif

ifeq ($(ARCH),linux-x64)
	CC		=gcc
	CFLAGS	+= -m64
endif

ifeq ($(ARCH),linux-x86)
	CC		=gcc
	CFLAGS	+= -m32
endif


ifeq ($(LIB),)

all-libs:
	for lib in $(ALL_LIBS); do	\
		$(MAKE) LIB=$${lib};	\
	done

endif


JPEG_SOURCES	=	$(wildcard ../$(LIB)/*.c)
JPEG_OBJECTS	=	$(addprefix $(ARCH)/$(LIB)/, $(notdir $(JPEG_SOURCES:.c=.o)))


all:	resources $(ARCH)/$(LIB).so


resources:
	rm -rf $(ARCH)/$(LIB)
	mkdir -p $(ARCH)/$(LIB)


$(ARCH)/$(LIB).so: $(ARCH)/$(LIB)/$(LIB).o $(JPEG_OBJECTS)
	$(CC) $(CFLAGS) -o $@ $^
	strip $@


$(ARCH)/$(LIB)/%.o: %.c
	$(CC) $(CFLAGS) -o $@ -c $<


$(ARCH)/$(LIB)/%.o: ../$(LIB)/%.c
	$(CC) $(CFLAGS) -o $@ -c $<


clean:
	for arch in $(ALL_ARCHS); do	\
		rm -rf $${arch};			\
	done
