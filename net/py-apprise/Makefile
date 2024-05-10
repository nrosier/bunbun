PORTNAME=	apprise
PORTVERSION=	1.7.6
CATEGORIES=	net python
MASTER_SITES=	PYPI
PKGNAMEPREFIX=	${PYTHON_PKGNAMEPREFIX}

MAINTAINER=	nick@bunbun.be
COMMENT=	Push notifications that work with just about every platform!
WWW=		https://pypi.org/project/apprise/

LICENSE=	BSD
LICENSE_FILE=	${WRKSRC}/LICENSE

BUILD_DEPENDS=  ${PY_SETUPTOOLS} \
                ${PYTHON_PKGNAMEPREFIX}wheel>0:devel/py-wheel@${PY_FLAVOR}

USES=		python
USE_PYTHON=	autoplist pep517 pytest

NO_ARCH=        yes

.include <bsd.port.mk>
