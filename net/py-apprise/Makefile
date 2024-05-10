PORTNAME=	apprise
PORTVERSION=	1.7.6
CATEGORIES=	net python
MASTER_SITES=	CHEESESHOP
PKGNAMEPREFIX=	${PYTHON_PKGNAMEPREFIX}

MAINTAINER=	nick@bunbun.be
COMMENT=	Push notifications that work with just about every platform!

LICENSE=	BSD

RUN_DEPENDS=	${PYTHON_PKGNAMEPREFIX}certifi>0:XXX/py-certifi \
			${PYTHON_PKGNAMEPREFIX}requests>0:XXX/py-requests \
			${PYTHON_PKGNAMEPREFIX}requests-oauthlib>0:XXX/py-requests-oauthlib \
			${PYTHON_PKGNAMEPREFIX}click>=5.0>0:XXX/py-click>=5.0 \
			${PYTHON_PKGNAMEPREFIX}markdown>0:XXX/py-markdown \
			${PYTHON_PKGNAMEPREFIX}PyYAML>0:XXX/py-PyYAML \
			${PYTHON_PKGNAMEPREFIX}dataclasses>0:XXX/py-dataclasses

USES=		python:3+ # 3, 3.6, 3.7, 3.8, 3.9, 3.10, 3.11, 3.12
USE_PYTHON=	autoplist distutils

.include <bsd.port.mk>
