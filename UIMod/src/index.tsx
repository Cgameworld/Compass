import React, { useState, useEffect } from 'react';
import { ModRegistrar } from "cs2/modding";
import { bindValue, trigger, useValue } from "cs2/api";
import ReactDOM from 'react-dom';
import SliderMod from './slider_modified';
import engine from 'cohtml/cohtml';
import { VanillaComponentsResolver } from '../types/internal';

const register: ModRegistrar = (moduleRegistry) => {

    const { DescriptionTooltip } = VanillaComponentsResolver.instance;

    const Rotation$ = bindValue<number>('Compass', 'Rotation');
    const CardinalDirectionMode$ = bindValue<boolean>('Compass', 'CardinalDirectionMode');

    const IsNorthAdjusted$ = bindValue<boolean>('Compass', 'IsNorthAdjusted');

    const correctAngle = (angleToCorrect: number): number => { 
        return Math.round(((angleToCorrect + 360) % 360));
    }

    const getDirection = (rotation: number): string => {
        const directions = ['N', 'NE', 'E', 'SE', 'S', 'SW', 'W', 'NW'];
        const normalizedRotation = correctAngle(rotation);
        const index = Math.round(normalizedRotation / 45) % 8;
        return directions[index];
    };

    interface CustomMenuButtonProps {
        editor?: boolean;
    }

    const CustomMenuButton: React.FC<CustomMenuButtonProps> = ({ editor = false }) => {
        const [showSettings, setShowSettings] = useState(false);
        const cardinalDirectionMode: boolean = useValue(CardinalDirectionMode$);

        const RotationNum: number = useValue(Rotation$);

        const toggleSettings = () => {
            setShowSettings(!showSettings);
            engine.trigger("audio.playSound", "select-item", 1);
        };

        useEffect(() => {
            if (showSettings) {
                const parentElement = editor ? document.querySelector('.main_k4u') : document.querySelector('.tutorial-renderer_wve.tutorials-renderer_uj6');
                if (parentElement) {
                    const settingsRoot = document.createElement('div');
                    settingsRoot.id = editor ? '' : 'top-right-layout_sSC';
                    parentElement.appendChild(settingsRoot);
                    ReactDOM.render(<SettingsWindow onClose={toggleSettings} cardinalDirectionMode={cardinalDirectionMode} editor={editor} />, settingsRoot);

                    return () => {
                        ReactDOM.unmountComponentAtNode(settingsRoot);
                        parentElement.removeChild(settingsRoot);
                    };
                }
            }
        }, [showSettings, cardinalDirectionMode]);

        const currentOrientation = correctAngle(useValue(Rotation$)) + "\u00b0 " + getDirection(RotationNum);
        const toolTipDescription = currentOrientation + " - Click to open options";

        // to indicate the 'general' selection state
        let cname = "button_ke4 button_h9N";
        if (editor) {
            cname = "button_FBo button_ECf item_It6 item-mouse-states_Fmi item-selected_tAM item-focused_FuT toggle-states_DTm button_FBo button_ECf item_It6 item-mouse-states_Fmi item-selected_tAM item-focused_FuT toggle-states_DTm item_IYJ";
        }
        if (showSettings) {
            cname += " selected";
        }
        return (
            <DescriptionTooltip title="Compass" description={toolTipDescription}>
                <button
                    className={cname}
                    onClick={toggleSettings}
                >
                    <div className="tinted-icon_iKo icon_be5" style={{
                        backgroundImage: 'url(coui://compassmod/FrameCircle.svg)',
                        backgroundColor: 'rgba(255,255,255,0)',
                        backgroundSize: editor ? '30rem 30rem' : '36rem 36rem',
                        position: 'relative',
                        width: editor ? '30rem' : '36rem',
                        height: editor ? '30rem' : '36rem',
                        filter: editor ? 'opacity(0.9)' : 'opacity(1)',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center'
                    }}>
                        {cardinalDirectionMode ? (
                            <div style={{
                                fontSize: '14rem',
                                fontWeight: 'bold',
                                color: 'white',
                                paddingTop: '1rem',
                                paddingRight: editor ? '1rem' : undefined
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
            </DescriptionTooltip>
        );
    };

    const SettingsWindow: React.FC<{ onClose: () => void, cardinalDirectionMode: boolean, editor: boolean }> = ({ onClose, cardinalDirectionMode, editor }) => {
        const toggleTextDir = () => {
            trigger("Compass", "SetCardinalDirectionMode", !cardinalDirectionMode);
            engine.trigger("audio.playSound", "select-item", 1);
        };

        const handleSliderInputChange = (newValue: number) => {
            trigger("Compass", "SetToAngle", newValue);
        };

        const RotationNum: number = correctAngle(useValue(Rotation$));
        const isNorth: boolean = RotationNum === 0 || RotationNum === 360;
        const isEast: boolean = RotationNum === 90;
        const isSouth: boolean = RotationNum === 180;
        const isWest: boolean = RotationNum === 270;


        let isNorthAdjustable = false;

        // to use the button_ke4 - defaults (hover, selected, ...)
        let cnameCardinalDirection = "button_ke4 button_h9N";
        if (cardinalDirectionMode) {
            cnameCardinalDirection += " selected";
        }

        let cnameN = "button_ke4 button_h9N";
        if (isNorth) {
            cnameN += " selected";
            // no need to make north to north...
            isNorthAdjustable = false;
        }
        let cnameE = "button_ke4 button_h9N";
        if (isEast) {
            cnameE += " selected";
            isNorthAdjustable = true;
        }
        let cnameS = "button_ke4 button_h9N";
        if (isSouth) {
            cnameS += " selected";
            isNorthAdjustable = true;
        }
        let cnameW = "button_ke4 button_h9N";
        if (isWest) {
            cnameW += " selected";
            isNorthAdjustable = true;
        }

        let isNorthAdjusted = useValue(IsNorthAdjusted$);

        return (
            <div className="panel_YqS expanded collapsible advisor-panel_dXi advisor-panel_mrr top-right-panel_A2r" style={{
                position: 'absolute',
                top: editor ? '120rem' : '50rem',
                left: editor ? '10rem' : undefined,
                right: editor ? undefined : '0rem',
                display: 'flex',
                width: '310rem',
                height: '250rem'
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
                                    <div className="row_S2v" style={{ paddingBottom: '10rem' }}>
                                        <div className="left_Lgw row_S2v" style={{ fontSize: '18rem', alignItems: 'center' }}>Cardinal Direction Mode</div>
                                        <div className="right_k3O row_S2v">
                                            <button
                                                className={cnameCardinalDirection}
                                                style={{
                                                    justifyContent: 'center',
                                                    color: 'white'
                                                }}
                                                onClick={toggleTextDir}
                                            >
                                                {cardinalDirectionMode ? 'On' : 'Off'}
                                            </button>
                                        </div>
                                    </div>
                                    <SliderMod title={"Heading"} min={0} max={360} sliderPos={RotationNum} onInputChange={handleSliderInputChange} />
                                    <div
                                        className="row_S2v"
                                        style={{
                                            paddingTop: '10rem',
                                            paddingBottom: '10rem',
                                            alignContent: 'center'
                                        }}>
                                        <DescriptionTooltip
                                            title="Adjust North"
                                            description="Make the current orientation to North for this Map">
                                            <button
                                                className="button_ke4 button_h9N"
                                                style={{
                                                    color: 'white',
                                                    width: '120rem',
                                                    alignContent: 'center',
                                                    visibility: isNorthAdjustable ? 'visible' : 'hidden'
                                                }}
                                                onClick={() => {
                                                    trigger("Compass", "MakeNorth");
                                                    engine.trigger("audio.playSound", "select-item", 1);
                                                }}>Make North</button>
                                        </DescriptionTooltip>
                                        <DescriptionTooltip
                                            title="Reset North-Adjustment"
                                            description="Reset North-Adjustment to Map-Defaults">
                                            <button
                                                className="button_ke4 button_h9N"
                                                style={{
                                                    color: 'white',
                                                    width: '120rem',
                                                    alignContent: 'center',
                                                    visibility: isNorthAdjusted ? 'visible' : 'hidden'
                                                }}
                                                onClick={() => {
                                                    trigger("Compass", "ResetNorth");
                                                    engine.trigger("audio.playSound", "select-item", 1);
                                                }}>Reset North</button>
                                        </DescriptionTooltip>
                                    </div>
                                    <div
                                        className="row_S2v"
                                        style={{
                                            paddingTop: '10rem',
                                            paddingBottom: '10rem',
                                            alignContent: 'center'
                                        }}>
                                        <DescriptionTooltip title="N" description="Set orientation to North">
                                            <button
                                                className={cnameN}
                                                style={{
                                                    justifyContent: 'center',
                                                    alignContent: 'center',
                                                    color: 'white'
                                                }}
                                                onClick={() => {
                                                    trigger("Compass", "SetToNorth");
                                                    engine.trigger("audio.playSound", "select-item", 1);
                                                    }}>N</button>
                                        </DescriptionTooltip>
                                        <DescriptionTooltip title="E" description="Set orientation to East">
                                            <button
                                                className={cnameE}
                                                style={{
                                                    justifyContent: 'center',
                                                    alignContent: 'center',
                                                    color: 'white'
                                                }}
                                                onClick={() => {
                                                    trigger("Compass", "SetToEast");
                                                    engine.trigger("audio.playSound", "select-item", 1);
                                                }}>E</button>
                                        </DescriptionTooltip>
                                        <DescriptionTooltip title="S" description="Set orientation to South">
                                            <button
                                                className={cnameS}
                                                style={{
                                                    justifyContent: 'center',
                                                    alignContent: 'center',
                                                    color: 'white'
                                                }}
                                                onClick={() => {
                                                    trigger("Compass", "SetToSouth");
                                                    engine.trigger("audio.playSound", "select-item", 1);
                                                }}>S</button>
                                        </DescriptionTooltip>
                                        <DescriptionTooltip title="W" description="Set orientation to West">
                                            <button
                                                className={cnameW}
                                                style={{
                                                    justifyContent: 'center',
                                                    alignContent: 'center',
                                                    color: 'white'
                                                }}
                                                onClick={() => {
                                                    trigger("Compass", "SetToWest");
                                                    engine.trigger("audio.playSound", "select-item", 1);
                                                }}>W</button>
                                        </DescriptionTooltip>
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

    const MapEditorButton: React.FC = () => {
        useEffect(() => {
            const targetContainer = document.querySelector('.content_XD5.content_AD7.child-opacity-transition_nkS.content_Hzl .row_B8G') as HTMLElement;

            if (targetContainer) {
                const newDiv = document.createElement('div');
                ReactDOM.render(
                    <div style={{ display: 'flex', flexDirection: 'row' }}><CustomMenuButton editor={true} />
                    </div>,
                    newDiv
                );

                targetContainer.insertBefore(newDiv, targetContainer.firstChild);
            }
        }, []);

        return null;
    };

    moduleRegistry.append('Editor', MapEditorButton);
};

export default register;