PJPROJECT_ROOT=../../submodules/pjproject

swig \
    -I${PJPROJECT_ROOT}/pjlib/include \
	-I${PJPROJECT_ROOT}/pjlib-util/include \
	-I${PJPROJECT_ROOT}/pjmedia/include \
	-I${PJPROJECT_ROOT}/pjsip/include \
	-I${PJPROJECT_ROOT}/pjnath/include \
    -I${PJPROJECT_ROOT}/pjsip/include \
    -c++ -w312 \
    -csharp -namespace org.pjsip.pjsua2 \
    -outcurrentdir \
    -outdir ../../org.pjsip.pjsua2/src \
    ${PJPROJECT_ROOT}/pjsip-apps/src/swig/pjsua2.i

mv -f pjsua2_wrap.h pjsua2_wrap.cxx ../../pjsua2_wrap/src
