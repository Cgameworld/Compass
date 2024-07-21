import React, { useState, useEffect } from 'react';
import { ModRegistrar } from "cs2/modding";
import { bindValue, trigger, useValue } from "cs2/api";
import ReactDOM from 'react-dom';
import SliderMod from './slider_modified';
import engine from 'cohtml/cohtml';

const register: ModRegistrar = (moduleRegistry) => {
    const Rotation$ = bindValue<number>('Compass', 'Rotation');

    const getDirection = (rotation: number): string => {
        const directions = ['N', 'NE', 'E', 'SE', 'S', 'SW', 'W', 'NW'];
        const normalizedRotation = ((rotation % 360) + 360) % 360;
        const index = Math.round(normalizedRotation / 45) % 8;
        return directions[index];
    };

    const CustomMenuButton: React.FC = () => {
        const [showSettings, setShowSettings] = useState(false);
        const [textDir, setTextDir] = useState(false);
        const RotationNum: number = useValue(Rotation$);

        const toggleSettings = () => {
            setShowSettings(!showSettings);
            engine.trigger("audio.playSound", "select-item", 1);
        };

        useEffect(() => {
            if (showSettings) {
                const tutorialRenderer = document.querySelector('.tutorial-renderer_wve.tutorials-renderer_uj6');
                if (tutorialRenderer) {
                    const settingsRoot = document.createElement('div');
                    settingsRoot.id = 'top-right-layout_sSC';
                    tutorialRenderer.appendChild(settingsRoot);
                    ReactDOM.render(<SettingsWindow onClose={toggleSettings} textDir={textDir} setTextDir={setTextDir} />, settingsRoot);

                    return () => {
                        ReactDOM.unmountComponentAtNode(settingsRoot);
                        tutorialRenderer.removeChild(settingsRoot);
                    };
                }
            }
        }, [showSettings, textDir]);

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

    const SettingsWindow: React.FC<{ onClose: () => void, textDir: boolean, setTextDir: (value: boolean) => void }> = ({ onClose, textDir, setTextDir }) => {
        const toggleTextDir = () => {
            setTextDir(!textDir);
            engine.trigger("audio.playSound", "select-item", 1);
        };

        const handleSliderInputChange = (newValue: number) => {
            trigger("Compass", "SetToAngle", newValue);
        };

        const RotationNum: number = Math.round((useValue(Rotation$) + 360) % 360);

        return (
            <div className="panel_YqS expanded collapsible advisor-panel_dXi advisor-panel_mrr top-right-panel_A2r" style={{
                position: 'absolute',
                top: '50rem',
                right: '0rem',
                display: 'flex',
                width: '310rem',
                height: '190rem'
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
                <div className="content_XD5 content_AD7 child-opacity-transition_nkS">
                    <div className="scrollable_DXr y_SMM scrollable_wt8">
                        <div className="content_gqa">
                            <div className="infoview-panel-section_RXJ">
                                <div className="content_1xS focusable_GEc item-focused_FuT">
                                    <div className="row_S2v" style={{paddingBottom:'10rem'}}>
                                        <div className="left_Lgw row_S2v" style={{ fontSize: '18rem', alignItems: 'center' }}>Cardinal Direction Mode</div>
                                        <div className="right_k3O row_S2v">
                                            <button
                                                className="button_WWa button_SH8"
                                                style={{
                                                    backgroundColor: textDir ? 'var(--selectedColor)' : 'var(--menuHoverColorBright)',
                                                    color: textDir ? 'white' : 'var(--menuText1Normal)'
                                                }}
                                                onClick={toggleTextDir}
                                            >
                                                {textDir ? 'On' : 'Off'}
                                            </button>
                                        </div>
                                    </div>
                                    <SliderMod title={"Bearing"} min={0} max={360} sliderPos={RotationNum} onInputChange={handleSliderInputChange} />                                   
                                    <div className="row_S2v" style={{ paddingTop: '10rem', paddingBottom: '10rem' }}>
                                        <button className="button_WWa button_SH8" style={{ justifyContent: 'center' }} onClick={() => {
                                            trigger("Compass", "SetToNorth");
                                            engine.trigger("audio.playSound", "select-item", 1);
                                        }}>Reset to North</button>
                                    </div>
                                    <div className="bottom-padding_JS3"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    };

    moduleRegistry.append('GameTopRight', CustomMenuButton);
};

export default register;