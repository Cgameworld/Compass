import engine from 'cohtml/cohtml';
import React, { useEffect, useRef, useState } from 'react'

interface SliderModProps {
    title: string | null;
    min: number;
    max: number;
    sliderPos: number;
    onInputChange: (value: number) => void;
}

const SliderMod: React.FC<SliderModProps> = ({ title, min, max, sliderPos, onInputChange }) => {
    const [sliderWidth, setSliderWidth] = useState<number>(0);
    const [inputValue, setInputValue] = useState<number>(sliderPos);
    const [isReady, setIsReady] = useState<boolean>(false);
    const sliderRef = useRef<HTMLDivElement>(null);
    const [scale, setScale] = useState<number>(1);
    const initialRenderRef = useRef<boolean>(true);

    const calculateSliderPosition = (currentWidth: number, value: number): number => {
        const newScale = (max - min) / currentWidth;
        return ((value - min) / newScale);
    };

    const updateSliderPosition = () => {
        if (sliderRef.current) {
            const currentWidth = sliderRef.current.getBoundingClientRect().width;
            if (currentWidth > 0) {
                const newScale = (max - min) / currentWidth;
                setScale(newScale);
                const newSliderWidth = calculateSliderPosition(currentWidth, sliderPos);
                setSliderWidth(Math.min(newSliderWidth, currentWidth));
                setInputValue(sliderPos);
                if (!isReady) {
                    setIsReady(true);
                }
            }
        }
    };

    // Initial setup using ResizeObserver
    useEffect(() => {
        if (sliderRef.current) {
            const resizeObserver = new ResizeObserver((entries) => {
                const entry = entries[0];
                if (entry && entry.contentRect.width > 0) {
                    const currentWidth = entry.contentRect.width;
                    const newScale = (max - min) / currentWidth;
                    setScale(newScale);
                    const newSliderWidth = calculateSliderPosition(currentWidth, sliderPos);
                    setSliderWidth(Math.min(newSliderWidth, currentWidth));
                    setInputValue(sliderPos);
                    if (!isReady) {
                        setIsReady(true);
                    }
                }
            });

            resizeObserver.observe(sliderRef.current);
            return () => resizeObserver.disconnect();
        }
    }, []);

    // Handle sliderPos updates
    useEffect(() => {
        if (!initialRenderRef.current) {
            updateSliderPosition();
        }
        initialRenderRef.current = false;
    }, [sliderPos]);

    const handleMouseDown = (e: React.MouseEvent<HTMLDivElement>) => {
        e.preventDefault();
        const handleMouseMove = (event: MouseEvent) => {
            if (sliderRef.current) {
                const currentWidth = sliderRef.current.getBoundingClientRect().width;
                const currentScale = (max - min) / currentWidth;
                setScale(currentScale);
                const newWidth = Math.min(
                    Math.max(event.clientX - sliderRef.current.getBoundingClientRect().left, 0),
                    currentWidth
                );
                setSliderWidth(newWidth);
                const newInputValue = Math.round(min + newWidth * currentScale);
                setInputValue(newInputValue);
                onInputChange(newInputValue);
                engine.trigger("audio.playSound", "drag-slider", 1);
            }
        };

        const handleMouseUp = () => {
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
        };

        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);
    };

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const value = event.target.value;
        if (value === "" || /^[0-9]+$/.test(value)) {
            const newValue = Number(value);
            setInputValue(newValue);
            if (newValue >= min && newValue <= max) {
                const currentWidth = sliderRef.current?.getBoundingClientRect().width || 0;
                const newSliderWidth = calculateSliderPosition(currentWidth, newValue);
                setSliderWidth(newSliderWidth);
                onInputChange(newValue);
            }
        }
    };

    return (
        <div className="row_d2o" style={{ paddingTop: '5rem' }}>
            <div className="left_Lgw row_S2v" style={{ fontSize: '18rem', alignItems: 'center' }}>{title}</div>
            <div className="right_k3O row_S2v" style={{ width: '75%', paddingRight: '5rem' }}>
                <div className="control_Hds" style={{ width: '67.5%', position: 'relative', left: '20rem' }}>
                    <div className="slider-container_Q_K" style={{ height: '10rem', opacity: isReady ? 1 : 0 }}>
                        <div className="slider_KXG slider_pUS horizontal slider_ROT">
                            <div className="track-bounds_H8_" ref={sliderRef}>
                                <div className="range-bounds_lNt" style={{ width: `${sliderWidth}px` }} onMouseDown={handleMouseDown}>
                                    <div className="range_nHO range_iUN"></div>
                                    <div className="thumb-container_aso">
                                        <div className="thumb_kkL"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <input
                    className="slider-input_DXM input_Wfi"
                    type="text"
                    value={inputValue}
                    onChange={handleInputChange}
                    style={{ marginLeft: '25rem', width: '20%' }}
                />
            </div>
        </div>
    );
};

export default SliderMod;