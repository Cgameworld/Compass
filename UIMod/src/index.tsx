import React, { useState, useEffect } from 'react';
import { ModRegistrar } from "cs2/modding";
import { bindValue, trigger, useValue } from "cs2/api";
import ReactDOM from 'react-dom';
import SliderMod from './components/slider_modified'
import ButtonMod from './components/button_modified';
import engine from 'cohtml/cohtml';
import { VanillaComponentsResolver } from '../types/internal';
import { useLocalization } from 'cs2/l10n';


const register: ModRegistrar = (moduleRegistry) => {

    const { DescriptionTooltip } = VanillaComponentsResolver.instance;

    // Translation.
    function translate(key: string) {
        const { translate } = useLocalization();
        return translate(key);
    }

    const Rotation$ = bindValue<number>('Compass', 'Rotation');
    const CardinalDirectionMode$ = bindValue<boolean>('Compass', 'CardinalDirectionMode');
    const RelativeNorthOffset$ = bindValue<number>('Compass', 'RelativeNorthOffset');

    let TitleBarLabel: string | null;
    let CardinalDirectionModeLabel: string | null;
    let OnLabel: string | null;
    let OffLabel: string | null;
    let SetLabel: string | null;
    let RelativeNorthLabel: string | null;
    let HeadingLabel: string | null;
    let ResetToRelativeNorthLabel: string | null;
    let ResetToNorthLabel: string | null;

    const getDirection = (rotation: number): string => {
        const directions = ['N', 'NE', 'E', 'SE', 'S', 'SW', 'W', 'NW'];
        const normalizedRotation = ((rotation % 360) + 360) % 360;
        const index = Math.round(normalizedRotation / 45) % 8;
        return directions[index];
    };

    interface CustomMenuButtonProps {
        editor?: boolean;
    }

    const CustomMenuButton: React.FC<CustomMenuButtonProps> = ({ editor = false }) => {
        TitleBarLabel = translate("Compass.TitleBarLabel");
        CardinalDirectionModeLabel = translate("Compass.CardinalDirectionModeLabel");
        OnLabel = translate("Compass.OnLabel");
        OffLabel = translate("Compass.OffLabel");
        SetLabel = translate("Compass.SetLabel");
        RelativeNorthLabel = translate("Compass.RelativeNorthLabel");
        HeadingLabel = translate("Compass.HeadingLabel");
        ResetToRelativeNorthLabel = translate("Compass.ResetToRelativeNorthLabel");
        ResetToNorthLabel = translate("Compass.ResetToNorthLabel");

        const [showSettings, setShowSettings] = useState(false);
        const cardinalDirectionMode: boolean = useValue(CardinalDirectionMode$);

        const RotationNum: number = useValue(Rotation$);
        const RelativeNorthOffset: number = Math.round(useValue(RelativeNorthOffset$));

        const AdjustedRotation = RotationNum - RelativeNorthOffset;

        const toggleSettings = () => {
            setShowSettings(!showSettings);
            engine.trigger("audio.playSound", "select-item", 1);
        };

        useEffect(() => {
            if (showSettings) {
                const parentElement = editor ? document.querySelector('.main_k4u') : document.querySelector('.main-container__E2');
                if (parentElement) {
                    const settingsRoot = document.createElement('div');
                    settingsRoot.id = editor ? '' : 'top-right-layout_sSC';
                    settingsRoot.style.width = '100%';
                    parentElement.appendChild(settingsRoot);
                    ReactDOM.render(<SettingsWindow onClose={toggleSettings} cardinalDirectionMode={cardinalDirectionMode} editor={editor} />, settingsRoot);

                    return () => {
                        ReactDOM.unmountComponentAtNode(settingsRoot);
                        parentElement.removeChild(settingsRoot);
                    };
                }
            }
        }, [showSettings, cardinalDirectionMode]);

        const currentOrientation = Math.round((useValue(Rotation$) + 360) % 360) + "\u00b0 " + getDirection(AdjustedRotation);
        const toolTipDescription = currentOrientation + " 11- " + translate("Compass.TooltipDescription");

        return (
            <DescriptionTooltip title={translate("Compass.TooltipTitle")} description={toolTipDescription}> 
                <button
                    id="MapTextureReplacer-MainGameButton"
                    className={editor ? "button_FBo button_ECf item_It6 item-mouse-states_Fmi item-selected_tAM item-focused_FuT toggle-states_DTm button_FBo button_ECf item_It6 item-mouse-states_Fmi item-selected_tAM item-focused_FuT toggle-states_DTm item_IYJ" : "button_ke4 button_h9N"}
                    onClick={toggleSettings}
                >
                    <div className="tinted-icon_iKo icon_be5" style={{
                        backgroundImage: 'url(coui://compassmod/FrameCircle.svg)',
                        backgroundColor: 'rgba(255,255,255,0)',
                        backgroundSize: editor ? '30rem 30rem' : '36rem 36rem',
                        position: 'relative',
                        width: editor ? '30rem' : '36rem',
                        height: editor ? '30rem' : '36rem',
                        filter : editor ? 'opacity(0.9)': 'opacity(1)',
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
                                {getDirection(AdjustedRotation)}
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
                                transform: `rotate(${AdjustedRotation}deg)`,
                                transformOrigin: 'center'
                            }} />
                        )}
                    </div> 
                </button>
            </DescriptionTooltip>
        );
    };

    const SettingsWindow: React.FC<{ onClose: () => void, cardinalDirectionMode: boolean, editor: boolean }> = ({ onClose, cardinalDirectionMode, editor }) => {
        
        const handleSliderInputChange = (newValue: number) => {
            /*
            console.table({
                newValue: newValue,
                RotationNum: RotationNum,
                RelativeNorthOffset: RelativeNorthOffset,
                TriggerSend: newValue + RelativeNorthOffset,
            });
            */
            trigger("Compass", "SetToAngle", newValue + RelativeNorthOffset);
        };

        const RotationNum: number = Math.round((useValue(Rotation$) + 360) % 360);
        const RelativeNorthOffset: number = Math.round(useValue(RelativeNorthOffset$));

        return (
            <div className="panel_YqS expanded collapsible advisor-panel_dXi advisor-panel_mrr top-right-panel_A2r" style={{
                position: 'absolute',
                top: editor ? '120rem' : '60rem',
                left: editor ? '10rem' : undefined,
                right: editor ? undefined : '0rem',
                display: 'flex',
                width: '340rem',
                height: '245rem'
            }}>
                <div className="header_H_U header_Bpo child-opacity-transition_nkS">
                    <div className="title-bar_PF4">
                        <div className="icon-space_h_f"></div>
                        <div className="title_SVH title_zQN">{TitleBarLabel}</div>
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
                                        <div className="left_Lgw row_S2v" style={{ fontSize: '18rem', alignItems: 'center' }}>{CardinalDirectionModeLabel}</div>
                                        <div className="right_k3O row_S2v">
                                            <ButtonMod active={cardinalDirectionMode} inactiveText={OffLabel} text={OnLabel} onClick={() => trigger("Compass", "SetCardinalDirectionMode", !cardinalDirectionMode)}/>
                                        </div>
                                    </div>
                                    <div className="row_S2v" style={{ paddingBottom: '10rem' }}>
                                        <div className="left_Lgw row_S2v" style={{ fontSize: '18rem', alignItems: 'center' }}>{RelativeNorthLabel}</div>
                                        <div className="right_k3O row_S2v">
                                            <ButtonMod active={RelativeNorthOffset !== 0} inactiveText={SetLabel} text={(RelativeNorthOffset > 0 ? "+" : "") + RelativeNorthOffset + "\u00b0"} onClick={() => {
                                                let adjustedRotationNum = RotationNum;
                                                if (RotationNum > 180) {
                                                    adjustedRotationNum = RotationNum - 360;
                                                }
                                                trigger("Compass", "SetRelativeNorthOffset", adjustedRotationNum);
                                            }} /> 
                                            <button className="button_WWa button_SH8" style={{ justifyContent: 'center',
                                            paddingLeft: '20rem',
                                            paddingRight: '20rem'    
                                            }}                                                onClick={() => {
                                                trigger("Compass", "SetRelativeNorthOffset", 0)
                                                engine.trigger("audio.playSound", "select-item", 1);
                                            }}>Reset</button>
                                        </div>
                                    </div>
                                    <SliderMod title={HeadingLabel} min={0} max={359} sliderPos={(RotationNum - RelativeNorthOffset + 360) % 360} onInputChange={handleSliderInputChange} />                                   
                                    <div className="row_S2v" style={{ paddingTop: '10rem', paddingBottom: '10rem' }}>
                                        <button className="button_WWa button_SH8" style={{ justifyContent: 'center' }} onClick={() => {
                                            trigger("Compass", "SetToNorth");
                                            engine.trigger("audio.playSound", "select-item", 1);
                                        }}>{RelativeNorthOffset !== 0 ? ResetToRelativeNorthLabel : ResetToNorthLabel}</button>
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
        const [mounted, setMounted] = useState(false);

        useEffect(() => {
            const targetContainer = document.querySelector('.content_XD5.content_AD7.child-opacity-transition_nkS.content_Hzl .row_B8G');
            if (targetContainer) {
                setMounted(true);
            }
        }, []);

        if (!mounted) return null;

        return ReactDOM.createPortal(
            <div style={{ display: 'flex', flexDirection: 'row' }}>
                <CustomMenuButton editor={true} />
            </div>,
            document.querySelector('.content_XD5.content_AD7.child-opacity-transition_nkS.content_Hzl .row_B8G')!
        );
    };

    moduleRegistry.append('Editor', MapEditorButton);
};

export default register;