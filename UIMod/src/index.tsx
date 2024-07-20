import React from 'react';
import { ModRegistrar } from "cs2/modding";
import { bindValue, trigger, useValue } from "cs2/api";

const register: ModRegistrar = (moduleRegistry) => {
    const Rotation$ = bindValue<number>('Compass', 'Rotation');

    const CustomMenuButton = () => {
        const RotationNum: number = useValue(Rotation$);

        return (
            <div>
                <button
                    id="MapTextureReplacer-MainGameButton"
                    className="button_ke4 button_ke4 button_h9N"
                    onClick={() => trigger("Compass", "SetRotation")}
                >
                    <div className="tinted-icon_iKo icon_be5" style={{
                        backgroundImage: 'url(coui://compassmod/FrameCircle.svg)',
                        backgroundColor: 'rgba(255,255,255,0)',
                        backgroundSize: '36rem 36rem',
                        position: 'relative',
                        width: '36rem',
                        height: '36rem'
                    }}>
                        <div className="tinted-icon_iKo icon_be5" style={{
                            position: 'absolute',
                            top: 0,
                            left: 0,
                            width: '100%',
                            height: '100%',
                            backgroundImage: 'url(coui://compassmod/CompassNeedle.svg)',
                            backgroundColor: 'rgba(255,255,255,0)',
                            backgroundSize: '100% 100%',
                            transform: `rotate(${RotationNum}deg)`,
                            transformOrigin: 'center'
                        }} />
                    </div>
                </button>
            </div>
        );
    }

    moduleRegistry.append('GameTopRight', CustomMenuButton);
}

export default register;