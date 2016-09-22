# Created by: Alex Dupre <ale@FreeBSD.org>
# $FreeBSD: head/print/pecl-pdflib/Makefile 419514 2016-08-03 12:47:05Z mat $

PORTNAME=	pdflib
PORTVERSION=	4.0.0
PORTREVISION=	0
CATEGORIES=	print pear
MASTER_SITES=	http://pecl.php.net/get/
PKGNAMEPREFIX=	pecl-
EXTRACT_SUFX=	.tgz
DIST_SUBDIR=	PECL

MAINTAINER=	ale@FreeBSD.org
COMMENT=	PECL extension to create PDF on the fly

LICENSE=	PHP30

BROKEN_powerpc64=	Does not build

LIB_DEPENDS=	libpdf.so:print/pdflib

USES=		php:ext
USE_PHP=	gd:build
PHP_MODNAME=	pdf

CONFIGURE_ARGS=	--with-pdflib=${LOCALBASE}

.include <bsd.port.mk>
