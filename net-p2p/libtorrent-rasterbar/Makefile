# Created by: Doug Barton <dougb@FreeBSD.org>
# $FreeBSD: head/net-p2p/libtorrent-rasterbar/Makefile 519824 2019-12-11 17:53:48Z jbeich $

PORTNAME=	libtorrent-rasterbar
DISTVERSIONPREFIX=	libtorrent-
DISTVERSION=	1_2_2
CATEGORIES=	net-p2p

MAINTAINER=	yuri@FreeBSD.org
COMMENT=	C++ library implementing a BitTorrent client

LICENSE=	BSD3CLAUSE
LICENSE_FILE=	${WRKSRC}/COPYING

LIB_DEPENDS=	libboost_chrono.so:devel/boost-libs \
		libboost_random.so:devel/boost-libs \
		libboost_system.so:devel/boost-libs

USES=		cmake compiler:c++14-lang iconv:wchar_t libtool pathfix pkgconfig python:2.7,test shebangfix ssl
LDFLAGS+=	-L${LOCALBASE}/lib ${ICONV_LIB}
SHEBANG_FILES=	test/socks.py test/web_server.py
USE_GITHUB=	yes
GH_ACCOUNT=	avidn
GH_PROJECT=	libtorrent
USE_LDCONFIG=	yes
TEST_TARGET=	test

SHLIB_VER=	10
PLIST_SUB+=	SHLIB_VER="${SHLIB_VER}"

DOCFILES=	AUTHORS ChangeLog
PORTDOCS=	${DOCFILES} docs/*
PORTEXAMPLES=	*.cpp

OPTIONS_DEFINE=	DOCS EXAMPLES LOGGING TEST
OPTIONS_DEFAULT=LOGGING
OPTIONS_SUB=	yes
LOGGING_DESC=	Enable logging to disk

EXAMPLES_CMAKE_BOOL=	build_examples
LOGGING_CMAKE_BOOL=	logging
TEST_USES=	python
TEST_CMAKE_BOOL=	build_tests

post-patch:
	@${REINPLACE_CMD} -e 's|/usr/local/include|${PREFIX}/include|' \
		${WRKSRC}/Jamfile
	@${REINPLACE_CMD} -e 's|python|${PYTHON_CMD}|' \
		${WRKSRC}/test/setup_transfer.cpp

post-install:
	@${RM} ${STAGEDIR}${PREFIX}/include/libtorrent/Makefile*
	# Workaround for -DTHREADS_HAVE_PTHREAD_ARG:BOOL=YES being set in
	# Mk/Uses/cmake.mk
	# References:   https://mariadb.atlassian.net/browse/MDEV-9388
	#               https://bugs.freebsd.org/bugzilla/show_bug.cgi?id=206169
	@${REINPLACE_CMD} 's/-l-pthread/-lpthread/' \
		${STAGEDIR}${PREFIX}/libdata/pkgconfig/libtorrent-rasterbar.pc

post-install-DOCS-on:
	@${MKDIR} ${STAGEDIR}${DOCSDIR}
	${INSTALL_DATA} ${DOCFILES:S|^|${WRKSRC}/|} ${STAGEDIR}${DOCSDIR}
	cd ${WRKSRC} && ${COPYTREE_SHARE} docs ${STAGEDIR}${DOCSDIR}
	${RM} ${STAGEDIR}${DOCSDIR}/docs/._*

post-install-EXAMPLES-on:
	@${MKDIR} ${STAGEDIR}${EXAMPLESDIR}
	${INSTALL_DATA} ${WRKSRC}/examples/${PORTEXAMPLES} \
		${STAGEDIR}${EXAMPLESDIR}

.include <bsd.port.mk>
