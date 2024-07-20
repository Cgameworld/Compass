import React, { useState, useEffect } from 'react';
import { ModRegistrar } from "cs2/modding";
import { bindValue, trigger, useValue } from "cs2/api";
import ReactDOM from 'react-dom';

const register: ModRegistrar = (moduleRegistry) => {
    const Rotation$ = bindValue<number>('Compass', 'Rotation');
    const textDir = false; // toggle relative text direction

    const getDirection = (rotation: number): string => {
        const directions = ['N', 'NE', 'E', 'SE', 'S', 'SW', 'W', 'NW'];
        const normalizedRotation = ((rotation % 360) + 360) % 360;
        const index = Math.round(normalizedRotation / 45) % 8;
        return directions[index];
    };

    const CustomMenuButton: React.FC = () => {
        const [showSettings, setShowSettings] = useState(false);
        const RotationNum: number = useValue(Rotation$);

        const toggleSettings = () => {
            setShowSettings(!showSettings);
        };

        useEffect(() => {
            if (showSettings) {
                const tutorialRenderer = document.querySelector('.tutorial-renderer_wve.tutorials-renderer_uj6');
                if (tutorialRenderer) {
                    const settingsRoot = document.createElement('div');
                    settingsRoot.id = 'top-right-layout_sSC';
                    tutorialRenderer.appendChild(settingsRoot);
                    ReactDOM.render(<SettingsWindow onClose={toggleSettings} />, settingsRoot);

                    return () => {
                        ReactDOM.unmountComponentAtNode(settingsRoot);
                        tutorialRenderer.removeChild(settingsRoot);
                    };
                }
            }
        }, [showSettings]);

        return (
            <div>
                <button
                    id="MapTextureReplacer-MainGameButton"
                    className="button_ke4 button_h9N"
                    onClick={toggleSettings}
                >
                    <div className="tinted-icon_iKo icon_be5" style={{
                        backgroundImage: 'url(coui://compassmod/FrameCircle.svg)',
                        backgroundColor: 'rgba(255,255,255,0)',
                        backgroundSize: '36rem 36rem',
                        position: 'relative',
                        width: '36rem',
                        height: '36rem',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center'
                    }}>
                        {textDir ? (
                            <div style={{
                                fontSize: '14rem',
                                fontWeight: 'bold',
                                color: 'white',
                                paddingTop: '1rem'
                            }}>
                                {getDirection(RotationNum)}
                            </div>
                        ) : (
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
                        )}
                    </div>
                </button>
            </div>
        );
    };

    const SettingsWindow: React.FC<{ onClose: () => void }> = ({ onClose }) => {
        return (
            <div className="panel_YqS expanded collapsible advisor-panel_dXi advisor-panel_mrr top-right-panel_A2r" style={{
                position: 'absolute',
                top: '50rem',
                right: '0rem',
                display: 'flex',
                width: '400rem',
                height: '400rem'
            }}>
                <div className="header_H_U header_Bpo child-opacity-transition_nkS">
                    <div className="title-bar_PF4">
                        <div className="icon-space_h_f"></div>
                        <div className="title_SVH title_zQN">Compass Settings</div>
                        <button className="button_bvQ button_bvQ close-button_wKK" onClick={onClose}>
                            <div className="tinted-icon_iKo icon_PhD" style={{ maskImage: 'url(Media/Glyphs/Close.svg)' }}></div>
                        </button>
                    </div>
                </div>
                
            </div>
        );
    };

    moduleRegistry.append('GameTopRight', CustomMenuButton);
};

export default register;